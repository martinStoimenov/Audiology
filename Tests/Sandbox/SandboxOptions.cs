namespace Sandbox
{
    using CommandLine;

    [Verb("sandbox", HelpText = "Run sandbox code.")]
    public static class SandboxOptions
    {
        public static void Main()
        {
            string name = "Palm Trees.mp3";

            var dotIndex = name.LastIndexOf('.');

            var fileExtension = name.Substring(dotIndex);

            var originalFileName = name.Substring(0, dotIndex);

            System.Console.WriteLine(originalFileName);
        }
    }
}
