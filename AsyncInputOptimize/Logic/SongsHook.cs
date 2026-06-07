using UnityEngine;

namespace AsyncInputOptimize.Logic
{
    public static class SongsHook
    {
        private static ulong GetAudioTime(AudioSource source) => (ulong)(source.timeSamples * 10_000_000.0 / ((long)(source.pitch * 1000000) / 1000000.0) / source.clip.frequency);
        public static void ResetTime()
        {
            SongsData.calibration = true;
            SongsData.currFrameTick = (ulong)CppBrige.GetSystemTick();
        }
        public static void PauseTime()
        {
            SongsData.currFrameTick = (ulong)CppBrige.GetSystemTick();
            scrConductor cdtr = scrConductor.instance;

            if (cdtr?.song?.clip != null)
                SongsData.song1OffsetTick = (SongsData.song1OffsetTick + SongsData.currFrameTick - GetAudioTime(cdtr.song)) >> 1;
            if (cdtr?.song2?.clip != null)
                SongsData.song2OffsetTick = (SongsData.song2OffsetTick + SongsData.currFrameTick - GetAudioTime(cdtr.song2)) >> 1;
        }
        public static void CountdownUpdate()
        {
            scrController ctrl = scrController.instance;
            scrConductor cdtr = scrConductor.instance;
            SongsData.currFrameTick = (ulong)CppBrige.GetSystemTick();
            if (SongsData.calibration)
            {
                if (cdtr.song != null && cdtr.song.isPlaying)
                {
                    SongsData.song1OffsetTick = SongsData.currFrameTick - GetAudioTime(cdtr.song);
                    SongsData.calibration = false;
                }
                if (cdtr.song2 != null && cdtr.song2.isPlaying)
                {
                    SongsData.song2OffsetTick = SongsData.currFrameTick - GetAudioTime(cdtr.song2);
                    SongsData.calibration = false;
                }
            }
            if (ctrl.goShown || cdtr.fastTakeoff || !(ctrl.state == States.PlayerControl || ctrl.state == States.Countdown || ctrl.state == States.Checkpoint))
                return;
            if (cdtr.song != null && cdtr.song.isPlaying)
                SongsData.song1OffsetTick = (SongsData.song1OffsetTick + SongsData.currFrameTick - GetAudioTime(cdtr.song)) >> 1;
            if (cdtr.song2 != null && cdtr.song2.isPlaying)
                SongsData.song2OffsetTick = (SongsData.song2OffsetTick + SongsData.currFrameTick - GetAudioTime(cdtr.song2)) >> 1;
        }
        public static void ConductorUpdate(scrConductor @this)
        {
            SongsData.currFrameTick = (ulong)CppBrige.GetSystemTick();

            if (scrController.instance != null && (!scrController.instance.goShown || SongsData.calibration))
            {
                CountdownUpdate();
                return;
            }
            if (AsyncInputManager.isActive)
            {
                if ((SongsData.song1OffsetTick | SongsData.song2OffsetTick) == 0)
                    ResetTime();
                if (scrController.instance?.paused ?? true)
                {
                    PauseTime();
                }
                double audio_precise = SafeDSPTime.GetAuidoPrecise();
                if (@this.song != null && @this.song.isPlaying)
                {
                    ulong offset_tick = SongsData.song1OffsetTick;
                    SongsData.song1OffsetTick_REAL = SongsData.currFrameTick - GetAudioTime(@this.song);
                    offset_tick = (offset_tick + SongsData.song1OffsetTick_REAL) >> 1;
                    long delta = (long)offset_tick - (long)SongsData.song1OffsetTick;
                    if (System.Math.Abs(delta) > audio_precise * 10000000 * 3)
                    {
                        EntryPoint.logger.Warning("Song1 XRUN Error");
                        @this.song.timeSamples += (int)(delta * @this.song.clip.frequency / 10_000_000) + (int)(audio_precise * @this.song.clip.frequency * SongsData.debug_multiply);
                    }
                }
                if (@this.song2 != null && @this.song2.isPlaying)
                {
                    ulong offset_tick = SongsData.song2OffsetTick_REAL;
                    SongsData.song2OffsetTick_REAL = SongsData.currFrameTick - GetAudioTime(@this.song2);
                    offset_tick = (offset_tick + SongsData.song2OffsetTick_REAL) >> 1;
                    long delta = (long)offset_tick - (long)SongsData.song2OffsetTick;
                    if (System.Math.Abs(delta) > audio_precise * 10000000 * 3)
                    {
                        EntryPoint.logger.Warning("Song2 XRUN Error");
                        @this.song2.timeSamples += (int)(delta * @this.song2.clip.frequency / 10_000_000) + (int)(audio_precise * @this.song2.clip.frequency * SongsData.debug_multiply);
                    }
                }
            }
        }
    }
}
