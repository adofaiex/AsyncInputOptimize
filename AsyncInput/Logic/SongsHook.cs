using ModsTagLib;
using UnityEngine;

namespace AsyncInput.Logic
{
    public static unsafe class SongsHook
    {
        private static ulong GetAudioTime(AudioSource source) => (ulong)(source.timeSamples * 10_000_000.0 / ((long)(source.pitch * 1000000) / 1000000.0) / source.clip.frequency);
        public static void ResetTime()
        {
            scrConductor cdtr = ADOBase.conductor;
            SongsData.currFrameTick = (ulong)ModsTagCLib.PreciseFileTime();
            if (cdtr == null) return;
            if (cdtr.song != null && cdtr.song.isPlaying)
                SongsData.song1OffsetTick = SongsData.currFrameTick - GetAudioTime(cdtr.song);
            if (cdtr.song2 != null && cdtr.song2.isPlaying)
                SongsData.song2OffsetTick = SongsData.currFrameTick - GetAudioTime(cdtr.song2);
        }
        public static void CountdownUpdate()
        {
            scrController ctrl = scrController.instance;
            scrConductor cdtr = scrConductor.instance;
            SongsData.currFrameTick = (ulong)ModsTagCLib.PreciseFileTime();
            if (ctrl.goShown || cdtr.fastTakeoff || !(ctrl.state == States.PlayerControl || ctrl.state == States.Countdown || ctrl.state == States.Checkpoint))
                return;
            if (cdtr.song != null && cdtr.song.isPlaying)
                SongsData.song1OffsetTick = (SongsData.song1OffsetTick + SongsData.currFrameTick - GetAudioTime(cdtr.song)) >> 1;
            if (cdtr.song2 != null && cdtr.song2.isPlaying)
                SongsData.song2OffsetTick = (SongsData.song2OffsetTick + SongsData.currFrameTick - GetAudioTime(cdtr.song2)) >> 1;
        }
        public static void ConductorUpdate(scrConductor @this)
        {
            SongsData.currFrameTick = (ulong)ModsTagCLib.PreciseFileTime();

            if (scrController.instance != null && !scrController.instance.goShown)
            {
                CountdownUpdate();
                return;
            }
            if (AsyncInputManager.isActive)
            {
                if ((SongsData.song1OffsetTick | SongsData.song2OffsetTick) == 0)
                    ResetTime();
                double audio_precise = ModsTagCLib.PreciseFileTime();
                if (@this.song != null && @this.song.isPlaying)
                {
                    ulong offset_tick = SongsData.song1OffsetTick_REAL;
                    SongsData.song1OffsetTick_REAL = SongsData.currFrameTick - GetAudioTime(@this.song);
                    offset_tick = (offset_tick + SongsData.song1OffsetTick_REAL) >> 1;
                    long delta = (long)offset_tick - (long)SongsData.song1OffsetTick;
                    if (System.Math.Abs(delta) > audio_precise * 10000000 * 3)
                    {
                        Starter.instance.log.WARN("Song1 XRUN Error");
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
                        Starter.instance.log.WARN("Song2 XRUN Error");
                        @this.song2.timeSamples += (int)(delta * @this.song2.clip.frequency / 10_000_000) + (int)(audio_precise * @this.song2.clip.frequency * SongsData.debug_multiply);
                    }
                }
            }
        }
    }
}
