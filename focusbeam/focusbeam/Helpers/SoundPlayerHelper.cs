/**
 * SoundPlayerHelper.cs
 * 
 * @author Prahlad Yeri <prahladyeri@yahoo.com>
 * @license MIT
 */
namespace focusbeam.Helpers
{
    public enum SystemSoundTheme
    {
        Asterisk,
        Beep,
        Exclamation,
        Hand,
        Question
    }

    public static class SoundPlayerHelper
    {
        public static void Play(SystemSoundTheme theme)
        {
            switch (theme)
            {
                case SystemSoundTheme.Asterisk:
                    System.Media.SystemSounds.Asterisk.Play();
                    break;
                case SystemSoundTheme.Beep:
                    System.Media.SystemSounds.Beep.Play();
                    break;
                case SystemSoundTheme.Exclamation:
                    System.Media.SystemSounds.Exclamation.Play();
                    break;
                case SystemSoundTheme.Hand:
                    System.Media.SystemSounds.Hand.Play();
                    break;
                case SystemSoundTheme.Question:
                    System.Media.SystemSounds.Question.Play();
                    break;
            }
        }
    }
}
