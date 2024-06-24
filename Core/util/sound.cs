using Box2DX.Collision;
using Newtonsoft.Json.Linq;
using System.Globalization;
using System.Reflection;
using System.Text.RegularExpressions;
using Xunit.Sdk;

namespace Core.util {

    using NAudio.Wave;
    using System;
    using System.Collections.Generic;

    public class Sound : IDisposable {

        public string FilePath { get; private set; }
        public float Volume { get; set; } = 1.0f;
        public bool Loop { get; set; } = false;

        private List<WaveOutEvent> _playingEvents;
        private List<AudioFileReader> _audioFileReaders;

        public Sound(string filePath, float volume = 1.0f, bool loop = false) {

            FilePath = filePath;
            Volume = volume;
            Loop = loop;
            _playingEvents = new List<WaveOutEvent>();
            _audioFileReaders = new List<AudioFileReader>();
        }

        public void Dispose() {

            foreach(var waveOut in _playingEvents)
                waveOut.Dispose();
            
            foreach(var reader in _audioFileReaders)
                reader.Dispose();
            
            _playingEvents.Clear();
            _audioFileReaders.Clear();
        }

        public void Play() {

            var audioFileReader = new AudioFileReader(FilePath) {
                Volume = Volume
            };
            var waveOutEvent = new WaveOutEvent();
            waveOutEvent.Init(audioFileReader);
            waveOutEvent.PlaybackStopped += (object sender, StoppedEventArgs e) => {

                audioFileReader.Dispose();
                waveOutEvent.Dispose();
                _playingEvents.Remove(waveOutEvent);
                _audioFileReaders.Remove(audioFileReader);
                if(Loop)
                    Play();
            };

            _playingEvents.Add(waveOutEvent);
            _audioFileReaders.Add(audioFileReader);
            waveOutEvent.Play();
        }

        public void Pause() {

            foreach(var waveOut in _playingEvents)
                waveOut.Pause();
        }

        public void Stop() {

            foreach(var waveOut in _playingEvents)
                waveOut.Stop();

            _playingEvents.Clear();
            _audioFileReaders.Clear();
        }
    }
}



/*

Fehler UnitTest.DropDown.Cellular_AutomataTests.Generate_Bit_Map_ShouldGenerateValidMap[18 ms]
  Fehlermeldung:
System.InvalidCastException : Unable to cast object of type 'System.Double' to type 'System.Int32'.
  Stapelverfolgung:
     at UnitTest.DropDown.Cellular_AutomataTests.Generate_Bit_Map_ShouldGenerateValidMap() in C:\Agents\Agent1\_work\3\s\UnitTest\DropDown\Cellular_Automata.cs:line 34
   at System.RuntimeMethodHandle.InvokeMethod(Object target, Void** arguments, Signature sig, Boolean isConstructor)
   at System.Reflection.MethodBaseInvoker.InvokeWithNoArgs(Object obj, BindingFlags invokeAttr)
  Fehler UnitTest.DropDown.Cellular_AutomataTests.Count_Ones_ShouldReturnCorrectCount [< 1 ms]
Fehlermeldung:
System.ArgumentException : Object of type 'System.Int32' cannot be converted to type 'System.UInt32'.
  Stapelverfolgung:
     at System.RuntimeType.CheckValue(Object & value, Binder binder, CultureInfo culture, BindingFlags invokeAttr)
   at System.Reflection.MethodBaseInvoker.InvokeWithOneArg(Object obj, BindingFlags invokeAttr, Binder binder, Object[] parameters, CultureInfo culture)
   at System.Reflection.MethodBase.Invoke(Object obj, Object[] parameters)
   at UnitTest.DropDown.Cellular_AutomataTests.Count_Ones_ShouldReturnCorrectCount() in C:\Agents\Agent1\_work\3\s\UnitTest\DropDown\Cellular_Automata.cs:line 127
   at System.RuntimeMethodHandle.InvokeMethod(Object target, Void** arguments, Signature sig, Boolean isConstructor)
   at System.Reflection.MethodBaseInvoker.InvokeWithNoArgs(Object obj, BindingFlags invokeAttr)
Ergebnisdatei: C:\Agents\Agent1\_work\_temp\FBE-SWENP1$_FBE-SWENP1_2024-06-24_20_51_33.trx

*/