using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CATHODE;
using CATHODE.Scripting;
using CathodeLib;

namespace AICustomScripts {

    class AICustomScripts {

        private static string rootDirectory = "F:/SteamLibrary/steamapps/common/Alien Isolation/";

        /*
         * Test of an own M18
         */
        public static void Main() {
            CustomM18.MainCustomM18(rootDirectory);
        }

        /*
         * Test of a custom mission with Stevieboy
         */
        public static void MainSomeTestStuff() {
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
            FunctionEntity displayMessage = composite.AddFunction(FunctionType.DisplayMessageWithCallbacks);
            displayMessage.AddParameter("title_text", new cString("HEY YOU!"));
            displayMessage.AddParameter("message_text", new cString("Do you like cookies?"));
            displayMessage.AddParameter("yes_text", new cString("yes"));
            displayMessage.AddParameter("no_text", new cString("no"));
            displayMessage.AddParameter("cancel_text", new cString("cancel"));
            displayMessage.AddParameter("yes_button", new cBool(true));
            displayMessage.AddParameter("no_button", new cBool(true));
            displayMessage.AddParameter("cancel_button", new cBool(true));

            FunctionEntity logicDelay = composite.AddFunction(FunctionType.LogicDelay);
            logicDelay.AddParameter("delay", new cFloat(3));
            logicDelay.AddParameterLink("on_delay_finished", displayMessage, "trigger");
            checkpoint.AddParameterLink("finished_loading", logicDelay, "trigger");

            // Add Stevieboy
            FunctionEntity steve = composite.AddFunction(commands.GetComposite("ARCHETYPES\\NPCS\\ALIEN\\XENOMORPH_NPC"));
            checkpoint.AddParameterLink("finished_loading", steve, "spawn_npc");

            commands.Save();
        }

        /*
         * Ask user a question, correct answer = happy, incorrect answer = kill player
         * @TODO - doesn't work yet (probably cause of the CMD_Die settings)
         */
        public static void MainTestWindow() {
            Commands commands = new Commands(rootDirectory + "DATA/ENV/PRODUCTION/ENG_TOWPLATFORM/WORLD/COMMANDS.PAK");
            Composite composite = commands.EntryPoints[0];
            FunctionEntity checkpoint = composite.AddFunction(FunctionType.Checkpoint);

            // Show first window
            FunctionEntity likeThisMod = composite.AddFunction(FunctionType.DisplayMessageWithCallbacks);
            likeThisMod.AddParameter("title_text", new cString("HEY YOU!"));
            likeThisMod.AddParameter("message_text", new cString("Do you like this mod?"));
            likeThisMod.AddParameter("yes_text", new cString("yes"));
            likeThisMod.AddParameter("no_text", new cString("no"));
            likeThisMod.AddParameter("yes_button", new cBool(true));
            likeThisMod.AddParameter("no_button", new cBool(true));

            // When clicked yes
            FunctionEntity goodBoy = composite.AddFunction(FunctionType.DisplayMessageWithCallbacks);
            goodBoy.AddParameter("title_text", new cString(":)"));
            goodBoy.AddParameter("message_text", new cString(""));
            goodBoy.AddParameter("yes_text", new cString("OK"));
            goodBoy.AddParameter("yes_button", new cBool(true));

            // When clicked no
            FunctionEntity triggerBindCharacter = composite.AddFunction(FunctionType.TriggerBindCharacter);
            FunctionEntity cmdDie = composite.AddFunction(FunctionType.CMD_Die);
            FunctionEntity variableThePlayer = composite.AddFunction(FunctionType.VariableThePlayer);

            triggerBindCharacter.AddParameterLink("characters", variableThePlayer, "reference");
            triggerBindCharacter.AddParameterLink("bound_trigger", cmdDie, "apply_start");
            likeThisMod.AddParameterLink("on_no", triggerBindCharacter, "trigger");

            FunctionEntity killPlayer = composite.AddFunction(FunctionType.CMD_Die);
            killPlayer.AddParameter("death_style", new cInteger(1));
            likeThisMod.AddParameterLink("on_no", killPlayer, "trigger");
            likeThisMod.AddParameterLink("on_yes", goodBoy, "trigger");

            // Displays the window after 5 seconds
            FunctionEntity logicDelayLikeThisMod = composite.AddFunction(FunctionType.LogicDelay);
            logicDelayLikeThisMod.AddParameter("delay", new cFloat(5));
            logicDelayLikeThisMod.AddParameterLink("on_delay_finished", likeThisMod, "trigger");
            checkpoint.AddParameterLink("finished_loading", logicDelayLikeThisMod, "trigger");

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
