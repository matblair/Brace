// Sound effect class. Code from:
// http://stevenpeirce.wordpress.com/2013/03/23/sharpdx-audio-in-windows-8-app/

using SharpDX.IO;
using SharpDX.Multimedia;
using SharpDX.XAudio2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brace.Utils
{
    public class SoundEffect
    {
        readonly XAudio2 _xaudio;
        readonly WaveFormat _waveFormat;
        readonly AudioBuffer _buffer;
        readonly SoundStream _soundstream;

        public SoundEffect(string soundFxPath, bool loop)
        {
            _xaudio = new XAudio2();
            var masteringsound = new MasteringVoice(_xaudio);

            var nativefilestream = new NativeFileStream(
                soundFxPath,
                NativeFileMode.Open,
                NativeFileAccess.Read,
                NativeFileShare.Read);

            _soundstream = new SoundStream(nativefilestream);
            _waveFormat = _soundstream.Format;
            _buffer = new AudioBuffer
            {
                Stream = _soundstream.ToDataStream(),
                AudioBytes = (int)_soundstream.Length,
                Flags = BufferFlags.EndOfStream,
                PlayBegin = 0,
                PlayLength = 0,
                LoopBegin = 0,
                LoopLength = 0,
                LoopCount = loop ? XAudio2.MaximumLoopCount : 0
            };
        }

        public void Play()
        {
            var sourceVoice = new SourceVoice(_xaudio, _waveFormat, true);
            sourceVoice.SubmitSourceBuffer(_buffer, _soundstream.DecodedPacketsInfo);
            sourceVoice.Start();
        }
    }
}
