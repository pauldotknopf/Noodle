using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PowerArgs;

namespace Noodle.Localization.Audio
{
    public class Arguements
    {
        [ArgDescription("The input language file to generate audio files for.")]
        [ArgShortcut("i")]
        [ArgExistingFile]
        public string InputFile { get; set; }

        [ArgDescription("The directory that the files will be generated into.")]
        [ArgShortcut("o")]
        [ArgExistingDirectory]
        public string OutputDirectory { get; set; }

        [ArgDescription("Should all the resources in the languages folder be deleted?")]
        [ArgShortcut("c")]
        public bool CleanOutputDirectory { get; set; }
    }
}
