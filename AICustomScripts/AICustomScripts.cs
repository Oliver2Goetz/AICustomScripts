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

        /*
         * Test of a custom mission with Stevieboy
         */
        public static void Main() {
            Commands commands = new Commands(rootDirectory + "DATA/ENV/PRODUCTION/ENG_TOWPLATFORM/WORLD/COMMANDS.PAK");
            Composite composite = commands.EntryPoints[0];

            // Add checkpoint
            FunctionEntity checkpoint = composite.AddFunction(FunctionType.Checkpoint);
            checkpoint.AddParameter("is_first_checkpoint", new cBool(true));
            checkpoint.AddParameter("section", new cString("Entry"));

            // Add position
            cTransform cTransform = new cTransform();
            cTransform.position = new CathodeLib.Vector3(0.1337f, 0.420f, 0.69f);
            cTransform.rotation = new CathodeLib.Vector3(0, 0, 0);
            checkpoint.AddParameter("position", cTransform);

            // Add player
            FunctionEntity playerSpawn = composite.AddFunction(commands.GetComposite("ARCHETYPES\\SCRIPT\\MISSION\\SPAWNPOSITIONSELECT"));
            checkpoint.AddParameterLink("finished_loading", playerSpawn, "SpawnPlayer");

            // Add objective
            FunctionEntity objective = composite.AddFunction(FunctionType.SetPrimaryObjective);
            objective.AddParameter("title", new cString("Survive!"));
            objective.AddParameter("additional_info", new cString("You should survive."));
            checkpoint.AddParameterLink("finished_loading", objective, "trigger");

            // Add a window after 3 seconds with 3 buttons
            FunctionEntity test = composite.AddFunction(FunctionType.DisplayMessageWithCallbacks);
            test.AddParameter("title_text", new cString("HEY YOU!"));
            test.AddParameter("message_text", new cString("Do you like cookies?"));
            test.AddParameter("yes_text", new cString("yes"));
            test.AddParameter("no_text", new cString("no"));
            test.AddParameter("cancel_text", new cString("cancel"));
            test.AddParameter("yes_button", new cBool(true));
            test.AddParameter("no_button", new cBool(true));
            test.AddParameter("cancel_button", new cBool(true));

            FunctionEntity logicDelay = composite.AddFunction(FunctionType.LogicDelay);
            logicDelay.AddParameter("delay", new cFloat(3));
            logicDelay.AddParameterLink("on_delay_finished", test, "trigger");
            checkpoint.AddParameterLink("finished_loading", logicDelay, "trigger");

            // Add Stevieboy
            FunctionEntity steve = composite.AddFunction(commands.GetComposite("ARCHETYPES\\NPCS\\ALIEN\\XENOMORPH_NPC"));
            checkpoint.AddParameterLink("finished_loading", steve, "spawn_npc");

            commands.Save();
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
