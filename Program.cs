using IGAE_GUI;
using IGAE_GUI.IGZ;

namespace igArchiveExtractor;

internal abstract class Program {
    private static void Main(string[] args) {
        if (args.Length == 0) OpenGui();
        else {
            var cmd = args[0];
            switch (cmd) {
                case "extractAll":
                    CmdExtractAll(args[1..]);
                    break;
                case "replaceFiles":
                    CmdReplaceFiles(args[1..]);
                    break;
                default: {
                    Console.WriteLine("Igae CLI 0.0.1");
                    Console.WriteLine("Igae 1.0.7f by Juni. Forked modifications and CLI by hydos");
                    Console.WriteLine("Available Commands:");
                    Console.WriteLine("extractAll <platform> <game> <igaFile> <extractLocation>");
                    Console.WriteLine("replaceFiles <platform> <game> <igaFile> <replacementDirectory> <outputIga>");
                    Console.WriteLine("help");
                    break;
                }
            }
        }
    }

    private static void CmdReplaceFiles(IReadOnlyList<string> strings) {
        if (strings.Count != 5) throw new Exception("Expected 5 arguments");
        var alchemyVersion = GetAlchemyIgaVersion(strings[0], strings[1]);
        var igaFile = new IGA_File(strings[2], alchemyVersion);
        var replacementDir = strings[3];
        var outputIgaLoc = strings[3];
        
        
    }

    private static void CmdExtractAll(IReadOnlyList<string> strings) {
        if (strings.Count != 4) throw new Exception("Expected 4 arguments");
        var alchemyVersion = GetAlchemyIgaVersion(strings[0], strings[1]);
        var igaFile = new IGA_File(strings[2], alchemyVersion);
        var extractDest = strings[3];

        for (uint i = 0; i < igaFile.numberOfFiles; i++) {
            igaFile.ExtractFile(i, extractDest, out var res);
            var name = igaFile.localFileHeaders[i].path;
            var result = res == 0 ? "succeeded" : "failed because file uses an unsupported compression method";
            Console.WriteLine($"Extracting file {name} {result}");
        }
    }

    private static IGA_Version GetAlchemyIgaVersion(string platform, string game) {
        IGA_Version alchemyVersion;
        switch (game) {
            case "ssa" when platform != "wiiu":
                alchemyVersion = IGA_Version.SkylandersSpyrosAdventureWii;
                break;
            case "ssa" when platform == "wiiu":
            case "sg":
                alchemyVersion = IGA_Version.SkylandersSpyrosAdventureWiiU;
                break;
            case "ssf":
                alchemyVersion = IGA_Version.SkylandersSwapForce;
                break;
            case "stt":
                alchemyVersion = IGA_Version.SkylandersTrapTeam;
                break;
            case "si" when platform == "ps4":
                alchemyVersion = IGA_Version.SkylandersImaginatorsPS4;
                break;
            case "sli":
                alchemyVersion = IGA_Version.SkylandersLostIslands;
                break;
            case "crash":
                alchemyVersion = IGA_Version.CrashNST;
                break;
            default:
                throw new Exception(
                    "Unable to figure out alchemy version :(. Make sure the platform is wii, wiiu, ps4, etc and game is ssa, sg, ssf, stt, ssc, si, sli, crash");
        }

        return alchemyVersion;
    }

    [STAThread]
    private static void OpenGui() {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new Form_igArchiveExtractor(Config.Read()));
    }
}