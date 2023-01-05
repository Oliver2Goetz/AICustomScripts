using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CATHODE;
using CATHODE.Scripting;
using CathodeLib;

namespace AICustomScripts {

    class CustomM18 {

        static Commands commands = null;
        static Composite composite = null;
        static FunctionEntity checkpoint = null;

        /*
         * Entrypoint of the custom mission
         */
        public static void MainCustomM18(string rootDirectory) {
            commands = new Commands(rootDirectory + "DATA/ENV/PRODUCTION/ENG_TOWPLATFORM/WORLD/COMMANDS.PAK");
            composite = commands.EntryPoints[0];

            // Setup for composite
            //setup();

            // Add checkpoint
            checkpoint = composite.AddFunction(FunctionType.Checkpoint);
            checkpoint.AddParameter("is_first_checkpoint", new cBool(true));
            checkpoint.AddParameter("section", new cString("Entry"));

            // Add player at M18 starting point
            FunctionEntity playerSpawn = composite.AddFunction(commands.GetComposite("ARCHETYPES\\SCRIPT\\MISSION\\SPAWNPOSITIONSELECT"));
            Vector3 position = new Vector3(28.2332f, -20.6875f, 13.4943f);
            Vector3 rotation = new Vector3(0, 180.0f, 0);
            playerSpawn.AddParameter("position", new cTransform(position, rotation));
            checkpoint.AddParameterLink("finished_loading", playerSpawn, "SpawnPlayer");

            // Objectives
            displayFirstObjective();

            commands.Save();
        }

        /*
         * Setup like mission clear, etc.
         */
        private static void setup() {
            Composite levelComposite = commands.GetComposite("SCRIPT_STORYMISSION\\M33_SHOWDOWN\\M33_PART_01\\M33_PT01");
            List<FunctionEntity> functions = levelComposite.functions;
            byte[] b = new byte[4] { 205, 248, 210, 1}; // Sound

            foreach (FunctionEntity function in functions) {
                if (function.function.val[0] == b[0]) {
                    functions.Remove(function);
                }
            }
        }

        /*
         * The first objective
         * The goal is to survive an alien wave for 3 minutes
         */
        private static void displayFirstObjective() {
            // Display objective message
            FunctionEntity objective = composite.AddFunction(FunctionType.SetPrimaryObjective);
            objective.AddParameter("title", new cString("Survive for 3 minutes!"));
            objective.AddParameter("additional_info", new cString("Your goal is to survive for 3 minutes."));
            checkpoint.AddParameterLink("finished_loading", objective, "trigger");

            // Give player flamethrower as soon as the first checkpoint loads
            FunctionEntity flamethrower = composite.AddFunction(FunctionType.WEAPON_GiveToPlayer);
            flamethrower.AddParameter("weapon", new cEnum(EnumType.EQUIPMENT_SLOT, 4));
            flamethrower.AddParameter("starting_ammo", new cInteger(250));
            checkpoint.AddParameterLink("finished_loading", flamethrower, "trigger");

            // Add Aliens
            FunctionEntity steve1 = composite.AddFunction(commands.GetComposite("ARCHETYPES\\NPCS\\ALIEN\\XENOMORPH_NPC"));
            checkpoint.AddParameterLink("finished_loading", steve1, "spawn_npc");
            FunctionEntity steve2 = composite.AddFunction(commands.GetComposite("ARCHETYPES\\NPCS\\ALIEN\\XENOMORPH_NPC"));
            checkpoint.AddParameterLink("finished_loading", steve2, "spawn_npc");
            FunctionEntity steve3 = composite.AddFunction(commands.GetComposite("ARCHETYPES\\NPCS\\ALIEN\\XENOMORPH_NPC"));
            checkpoint.AddParameterLink("finished_loading", steve3, "spawn_npc");
            FunctionEntity steve4 = composite.AddFunction(commands.GetComposite("ARCHETYPES\\NPCS\\ALIEN\\XENOMORPH_NPC"));
            checkpoint.AddParameterLink("finished_loading", steve4, "spawn_npc");
            FunctionEntity steve5 = composite.AddFunction(commands.GetComposite("ARCHETYPES\\NPCS\\ALIEN\\XENOMORPH_NPC"));
            checkpoint.AddParameterLink("finished_loading", steve5, "spawn_npc");

            // Display objective successfully completed message
            FunctionEntity objectiveSurvived = composite.AddFunction(FunctionType.SetPrimaryObjective);
            objectiveSurvived.AddParameter("title", new cString("Amazing! You survived 3 minutes."));
            objectiveSurvived.AddParameter("additional_info", new cString("You survived 3 minutes. Good job!"));

            // Start a timer for 3 minutes: If the time has been passed despawn all aliens and update objective
            FunctionEntity logicDelay3min = composite.AddFunction(FunctionType.LogicDelay);
            logicDelay3min.AddParameter("delay", new cFloat(30f)); // TODO Change to 3 minutes later (30seconds is just for early testing purposes)
            logicDelay3min.AddParameterLink("on_delay_finished", objectiveSurvived, "trigger");
            logicDelay3min.AddParameterLink("on_delay_finished", steve1, "despawn_npc");
            logicDelay3min.AddParameterLink("on_delay_finished", steve2, "despawn_npc");
            logicDelay3min.AddParameterLink("on_delay_finished", steve3, "despawn_npc");
            logicDelay3min.AddParameterLink("on_delay_finished", steve4, "despawn_npc");
            logicDelay3min.AddParameterLink("on_delay_finished", steve5, "despawn_npc");

            checkpoint.AddParameterLink("finished_loading", logicDelay3min, "trigger");
        }
    }
}
