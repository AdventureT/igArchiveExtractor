using System.Security.Cryptography.Pkcs;
using IGAE_GUI;
using IGAE_GUI.IGZ;

namespace igArchiveExtractor;

internal abstract class Program {
    private static void Main(string[] args) {
        if (args.Length == 0) OpenGui();
        else {
            Console.WriteLine("Igae CLI 0.0.1");
            Console.WriteLine("Igae 1.0.7f by Juni. Forked modifications and CLI by hydos");
            var cmd = args[0];
            switch (cmd) {
                case "extractAll":
                    CmdExtractAll(args[1..]);
                    break;
                case "modifyFiles":
                    CmdModifyFiles(args[1..]);
                    break;
                default: {
                    DisplayHelpMessage(args[1..]);
                    break;
                }
            }
        }
    }

    private static void DisplayHelpMessage(IReadOnlyList<string> args) {
        if (args.Count > 1) {
            switch (args[0]) {
                case "extractAll": {
                    Console.WriteLine("extractAll <platform> <game> <igaFile> <extractLocation>");
                    Console.WriteLine(
                        "<platform>: the platform you want to read from. For example: wii, wiiu, ps4, ps3, switch, etc");
                    Console.WriteLine(
                        "<game>: the shortened name of the game. For example: ssa, sg, ssf, stt, ssc, si, sli, crash");
                    Console.WriteLine("<igaFile>: the IGA you are trying to extract from");
                    Console.WriteLine("<extractLocation>: the location to extract the contents to");
                    break;
                }

                case "modifyFiles": {
                    Console.WriteLine(
                        "modifyFiles <platform> <game> <igaFile> <action> <replacementDirectory> <outputIga>");
                    Console.WriteLine(
                        "<platform>: the platform you want to read from. For example: wii, wiiu, ps4, ps3, switch, etc");
                    Console.WriteLine(
                        "<game>: the shortened name of the game. For example: ssa, sg, ssf, stt, ssc, si, sli, crash");
                    Console.WriteLine("<igaFile>: the IGA you are trying to modify");
                    Console.WriteLine(
                        "<action>: this setting does absolutely nothing due to limitations with igae");
                    break;
                }

                default:
                    Console.Error.WriteLine($"Couldn't help with the unknown command \"{args[0]}\"");
                    break;
            }
        }
        else {
            Console.WriteLine("Available Commands:");
            Console.WriteLine("extractAll <platform> <game> <igaFile> <extractLocation>");
            Console.WriteLine("modifyFiles <platform> <game> <igaFile> <action> <inputDirectory> <outputIga>");
            Console.WriteLine("help");
        }
    }

    private static void CmdModifyFiles(IReadOnlyList<string> strings) {
        if (strings.Count != 6) throw new Exception("Expected 6 arguments");
        var alchemyVersion = GetAlchemyIgaVersion(strings[0], strings[1]);
        var igaFile = new IGA_File(strings[2], alchemyVersion);
        var action = strings[3];
        var inputDirectory = strings[4];
        var outputIgaLoc = strings[5];

        var gamePaths = new string[igaFile.numberOfFiles];
        var computerPaths = new string[igaFile.numberOfFiles];

        for (var i = 0; i < igaFile.numberOfFiles; i++) {
            gamePaths[i] = igaFile.names[i];
            computerPaths[i] = Path.Join(inputDirectory, gamePaths[i]);
        }

        var outputFile = new IGA_File(alchemyVersion, gamePaths, igaFile.crc) {
            slop = igaFile.slop,
            flags = igaFile.flags
        };

        try {
            outputFile.Build(outputIgaLoc, computerPaths);
        }
        catch (Exception e) {
            Console.WriteLine("An error occured building the archive. Maybe you are missing a file?");
            Console.WriteLine(e);
            throw;
        }
        
        Console.WriteLine("It worked maybe? Who knows with this damn tool");
    }

    private static uint HashFileName(string name, uint basis = 0x811c9dc5) {
        name = name.ToLower().Replace('\\', '/');
        return name.Aggregate(basis, (current, t) => (current ^ t) * 0x1000193);
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