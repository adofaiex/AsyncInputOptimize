using ModsTagLib.Time;
using UnityEngine;

namespace AsyncInput.Logic
{
    public static unsafe class SongsHook
    {
        private static ulong GetAudioTime(AudioSource source) => (ulong)(source.timeSamples * (10_000_000.0 / ((long)(source.pitch * 1000000) / 1000000.0)) / source.clip.frequency);
        public static void ResetTime()
        {
            scrConductor cdtr = ADOBase.conductor;
            SongsData.currFrameTick = TimeInstance.PTime.U_Tick();
            if (cdtr == null) return;
            if (cdtr.song != null && cdtr.song.isPlaying)
                SongsData.song1OffsetTick = 0;
            if (cdtr.song2 != null && cdtr.song2.isPlaying)
                SongsData.song2OffsetTick = 0;
        }
        public static void CountdownUpdate()
        {
            scrController ctrl = scrController.instance;
            scrConductor cdtr = scrConductor.instance;
            SongsData.currFrameTick = TimeInstance.PTime.U_Tick();
            if (ctrl.goShown || cdtr.fastTakeoff || !(ctrl.state == States.PlayerControl || ctrl.state == States.Countdown || ctrl.state == States.Checkpoint))
                return;
            if (cdtr.song != null && cdtr.song.isPlaying)
            {
                SongsData.song1OffsetTick_REAL = SongsData.currFrameTick - GetAudioTime(cdtr.song);
                if (SongsData.song1OffsetTick == 0)
                    SongsData.song1OffsetTick = SongsData.song1OffsetTick_REAL;
                else
                    SongsData.song1OffsetTick = (SongsData.song1OffsetTick + SongsData.song1OffsetTick_REAL) >> 1;
            }
            if (cdtr.song2 != null && cdtr.song2.isPlaying)
            {
                SongsData.song2OffsetTick_REAL = SongsData.currFrameTick - GetAudioTime(cdtr.song2);
                if (SongsData.song2OffsetTick == 0)
                    SongsData.song2OffsetTick = SongsData.song2OffsetTick_REAL;
                else
                    SongsData.song2OffsetTick = (SongsData.song2OffsetTick + SongsData.song2OffsetTick_REAL) >> 1;
            }
        }
        public static void ConductorUpdate(scrConductor @this)
        {
            SongsData.currFrameTick = TimeInstance.PTime.U_Tick();

            if (scrController.instance != null && !scrController.instance.goShown)
            {
                CountdownUpdate();
                return;
            }
            if (!(AsyncInputManager.isActive && AsyncInputData.enabled))
                return;

            if ((SongsData.song1OffsetTick | SongsData.song2OffsetTick) == 0)
                ResetTime();

            double audio_precise = SafeDSPTime.GetAuidoPrecise();

            if (@this.song != null && @this.song.isPlaying)
            {
                ulong offset_tick = SongsData.song1OffsetTick_REAL;
                SongsData.song1OffsetTick_REAL = SongsData.currFrameTick - GetAudioTime(@this.song);
                offset_tick = (offset_tick + SongsData.song1OffsetTick_REAL) >> 1;
                long delta = (long)(offset_tick - SongsData.song1OffsetTick);
                if (delta > 0 && SongsData.song1_offset)
                    goto NEXT;
                else if (SongsData.song1_offset)
                    SongsData.song1_offset = false;

                if (System.Math.Abs(delta) <= audio_precise * 10000000 * 3)
                    goto NEXT;
                if (SwapArea.audioDelta != 0)
                {
                    Starter.instance.log.WARN("Song1 Error: " + (SwapArea.audioDelta / TimeConvert.D_Second_Nano));
                    @this.song.timeSamples -= (int)((SwapArea.audioDelta / TimeConvert.D_Second_Nano + audio_precise * SongsData.debug_multiply) * @this.song.clip.frequency);
                    SongsData.song1_offset = true;
                    SwapArea.audioDelta = 0;
                }
                else
                {
                    Starter.instance.log.WARN("Song1 Error: " + delta);
                    long value = (long)(SongsData.currFrameTick - SongsData.song1OffsetTick);
                    @this.song.timeSamples = (int)((value / TimeConvert.D_Second_Tick + audio_precise * SongsData.debug_multiply) * @this.song.clip.frequency);
                }
            }
        NEXT:
            if (@this.song2 != null && @this.song2.isPlaying)
            {
                ulong offset_tick = SongsData.song2OffsetTick_REAL;
                SongsData.song2OffsetTick_REAL = SongsData.currFrameTick - GetAudioTime(@this.song2);
                offset_tick = (offset_tick + SongsData.song2OffsetTick_REAL) >> 1;
                long delta = (long)(offset_tick - SongsData.song2OffsetTick);
                if (System.Math.Abs(delta) > audio_precise * 10000000 * 3)
                {
                    Starter.instance.log.WARN("Song2 Error: " + delta);
                    @this.song2.timeSamples += (int)(delta * @this.song2.clip.frequency / 10_000_000) + (int)(audio_precise * @this.song2.clip.frequency * SongsData.debug_multiply);
                }
            }
        EMD:
            return;
        }
    }
}
