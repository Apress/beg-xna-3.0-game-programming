using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml.Serialization;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace XNA_TPS.Helpers
{
    [Serializable]
    public struct KeyboardSettings
    {
        public Keys A;
        public Keys B;
        public Keys X;
        public Keys Y;
        public Keys LeftShoulder;
        public Keys RightShoulder;
        public Keys LeftTrigger;
        public Keys RightTrigger;
        public Keys LeftStick;
        public Keys RightStick;
        public Keys Back;
        public Keys Start;

        public Keys DPadDown;
        public Keys DPadLeft;
        public Keys DPadRight;
        public Keys DPadUp;

        public Keys LeftThumbstickDown;
        public Keys LeftThumbstickLeft;
        public Keys LeftThumbstickRight;
        public Keys LeftThumbstickUp;
        public Keys RightThumbstickDown;
        public Keys RightThumbstickLeft;
        public Keys RightThumbstickRight;
        public Keys RightThumbstickUp;
    }

    [Serializable]
    public struct GameSettings
    {
        public bool PreferredFullScreen;
        public int PreferredWindowWidth;
        public int PreferredWindowHeight;
        public bool EnableVsync;

        public KeyboardSettings[] KeyboardSettings;
    }

    public static class SettingsManager
    {
        public static GameSettings Read(string settingsFilename)
        {
            GameSettings gameSettings;
            Stream stream = File.OpenRead(settingsFilename);
            XmlSerializer serializer = new XmlSerializer(typeof(GameSettings));

            gameSettings = (GameSettings)serializer.Deserialize(stream);
            return gameSettings;
        }

        public static void Save(string settingsFilename, GameSettings gameSettings)
        {
            Stream stream = File.OpenWrite(settingsFilename);
            XmlSerializer serializer = new XmlSerializer(typeof(GameSettings));

            serializer.Serialize(stream, gameSettings);
        }

        public static Dictionary<Buttons, Keys> GetKeyboardDictionary(KeyboardSettings keyboard)
        {
            Dictionary<Buttons, Keys> dictionary = new Dictionary<Buttons, Keys>();

            dictionary.Add(Buttons.A, keyboard.A);
            dictionary.Add(Buttons.B, keyboard.B);
            dictionary.Add(Buttons.X, keyboard.X);
            dictionary.Add(Buttons.Y, keyboard.Y);

            dictionary.Add(Buttons.LeftShoulder, keyboard.LeftShoulder);
            dictionary.Add(Buttons.RightShoulder, keyboard.RightShoulder);
            dictionary.Add(Buttons.LeftTrigger, keyboard.LeftTrigger);
            dictionary.Add(Buttons.RightTrigger, keyboard.RightTrigger);
            dictionary.Add(Buttons.LeftStick, keyboard.LeftStick);
            dictionary.Add(Buttons.RightStick, keyboard.RightStick);
            dictionary.Add(Buttons.Back, keyboard.Back);
            dictionary.Add(Buttons.Start, keyboard.Start);

            dictionary.Add(Buttons.DPadDown, keyboard.DPadDown);
            dictionary.Add(Buttons.DPadLeft, keyboard.DPadLeft);
            dictionary.Add(Buttons.DPadRight, keyboard.DPadRight);
            dictionary.Add(Buttons.DPadUp, keyboard.DPadUp);

            dictionary.Add(Buttons.LeftThumbstickDown, keyboard.LeftThumbstickDown);
            dictionary.Add(Buttons.LeftThumbstickLeft, keyboard.LeftThumbstickLeft);
            dictionary.Add(Buttons.LeftThumbstickRight, keyboard.LeftThumbstickRight);
            dictionary.Add(Buttons.LeftThumbstickUp, keyboard.LeftThumbstickUp);
            dictionary.Add(Buttons.RightThumbstickDown, keyboard.RightThumbstickDown);
            dictionary.Add(Buttons.RightThumbstickLeft, keyboard.RightThumbstickLeft);
            dictionary.Add(Buttons.RightThumbstickRight, keyboard.RightThumbstickRight);
            dictionary.Add(Buttons.RightThumbstickUp, keyboard.RightThumbstickUp);

            return dictionary;
        }

    }
}
