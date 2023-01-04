using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CATHODE;
using CATHODE.Scripting;

namespace AICustomScripts {

    class AICustomScripts {

        private static string rootDirectory = "F:/SteamLibrary/steamapps/common/Alien Isolation/";

        public static void Main() {

        }

        /*
         * Using an existing COMMANDS.PAK and modify it (spawn player + add objective)
         */
        public static void MainEditExistingPAK() {
            Commands commands = new Commands(rootDirectory + "DATA/ENV/PRODUCTION/ENG_ALIEN_NEST/WORLD/COMMANDS.PAK");
            Composite composite = commands.EntryPoints[0];
            composite.functions.Clear();

            FunctionEntity checkpoint = composite.AddFunction(FunctionType.Checkpoint);
            FunctionEntity playerSpawn = composite.AddFunction(commands.GetComposite("ARCHETYPES\\SCRIPT\\MISSION\\SPAWNPOSITIONSELECT"));

            checkpoint.AddParameter("is_first_checkpoint", new cBool(true));
            checkpoint.AddParameter("section", new cString("Entry"));

            // Maybe add player position
            cTransform cTransform = new cTransform();
            cTransform.position = new CathodeLib.Vector3(0.1337f, 0.420f, 0.69f);
            cTransform.rotation = new CathodeLib.Vector3(0, 0, 0);
            playerSpawn.AddParameter("position", cTransform);

            checkpoint.AddParameterLink("finished_loading", playerSpawn, "SpawnPlayer");
            commands.Save();
        }

        /*
         * First example of creating a new COMMANDS.PAK file using CathodeLib
         */
        public static void MainCreateNewCommandsPak() {
            Commands commands = new Commands("COMMANDS.PAK");
            Composite composite = commands.AddComposite("My coolScript", true);

            FunctionEntity checkpoint = composite.AddFunction(FunctionType.Checkpoint);
            checkpoint.AddParameter("is_first_checkpoint", new cBool(true));
            checkpoint.AddParameter("section", new cString("Entry"));

            FunctionEntity objective = composite.AddFunction(FunctionType.SetPrimaryObjective);
            objective.AddParameter("title", new cString("Where Steve?"));
            objective.AddParameter("additional_info", new cString("Go and find Steve!"));

            checkpoint.AddParameterLink("finished_loading", objective, "trigger");

            commands.Save();
        }
    }
}
