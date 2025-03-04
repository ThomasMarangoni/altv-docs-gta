using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace VehiclePageCreator
{
    class Program
    {
        static void Main(string[] args)
        {
            var depsFolder = "../../deps/gta-v-data-dumps/";
            var vehiclesClassesJson = depsFolder + "vehiclesClasses.json";
            var vehiclesJson = depsFolder + "vehicles.json";

            var vehiclesModelsFile = "../../../articles/vehicle/models.md";
            var imagePath = "~/altv-docs-assets/altv-docs-gta/images/vehicle/models/";

            var skipDlcTag = "g9ec"; //Generation 9 Enhanced Content

            /*
             * Read JSON files from gta-v-data-dumps by DurtyFree
             */

            if (!File.Exists(vehiclesClassesJson))
            {
                Console.WriteLine("Couldn't find " + vehiclesClassesJson + ". Are you running the project with 'dotnet run .\\generators-vehicle_models.csproj'?");
                Environment.Exit(1);
            }

            if (!File.Exists(vehiclesJson))
            {
                Console.WriteLine("Couldn't find " + vehiclesJson + ". Are you running the project with 'dotnet run .\\generators-vehicle_models.csproj'?");
                Environment.Exit(2);
            }

            using var readerVehicleClasses = new StreamReader(vehiclesClassesJson);
            var jsonVehicleClasses = readerVehicleClasses.ReadToEnd();
            var vehicleClasses = JsonConvert.DeserializeObject<List<string>>(jsonVehicleClasses);

            using var readerVehicles = new StreamReader(vehiclesJson);
            var jsonVehicles = readerVehicles.ReadToEnd();
            var vehicles = JsonConvert.DeserializeObject<List<Vehicle>>(jsonVehicles);

            var sortedVehiclesByName = vehicles.OrderBy(x => x.Name).Where(x => !x.DlcName.Contains(skipDlcTag));
            var sortedVehiclesByNameSkipped = vehicles.OrderBy(x => x.Name).Where(x =>x.DlcName.Contains(skipDlcTag));

            /*
             * Generate Images
             */
            var gallery = File.CreateText(vehiclesModelsFile);

            gallery.WriteLine("<!--- THIS IS AN AUTOGENERATED FILE. DO NOT EDIT THIS FILE DIRECTLY. -->");
            gallery.WriteLine("<!--- This page gets generated with tools/deps/generators/vehicle_models -->");
            gallery.WriteLine("# Vehicle Models");

            gallery.WriteLine();
            gallery.WriteLine("## Gallery");
            gallery.WriteLine();
            
            gallery.WriteLine("### Categories");
            gallery.WriteLine();
            
            foreach (var vehicleClass in vehicleClasses)
            {
                gallery.WriteLine($"<a href=\"#{vehicleClass.ToLower()}\">{vehicleClass}</a><br/>");
            }
            gallery.WriteLine($"<a href=\"#others\">OTHERS</a><br/>");
            gallery.WriteLine();
            
            foreach (var vehicleClass in vehicleClasses)
            {
                gallery.WriteLine("### " + vehicleClass);
                gallery.WriteLine();
                gallery.WriteLine("<div class=\"grid-container\">");

                var vehiclesByClass = sortedVehiclesByName.Where(x => x.Class == vehicleClass);
                foreach (var vehicle in vehiclesByClass)
                {
                    gallery.WriteLine("  <div class=\"grid-item\">");

                    var vehPath = imagePath + vehicle.Name.ToLower();

                    gallery.WriteLine($"    <div class=\"grid-item-img\">");
                    gallery.WriteLine($"      <img src=\"{vehPath}.png\" alt=\"Missing image &quot;{vehicle.Name}.png&quot;\" title=\"{vehicle.Name}\" loading=\"lazy\" />");
                    gallery.WriteLine($"    </div>");

                    if (vehicle.DlcName.ToLower() == "titleupdate")
                    {
                        gallery.WriteLine("    <b>Name:</b> " + vehicle.Name + "<br/>");
                        gallery.WriteLine("    <b>Hash:</b> " + vehicle.HexHash + "<br/>");
                        gallery.WriteLine("    <b>Display Name:</b> " + vehicle.DisplayName );
                    }
                    else
                    {
                        gallery.WriteLine("    <b>Name:</b> " + vehicle.Name + "<br/>");
                        gallery.WriteLine("    <b>Hash:</b> " + vehicle.HexHash + "<br/>");
                        gallery.WriteLine("    <b>Display Name:</b> " + vehicle.DisplayName + "<br/>");
                        gallery.WriteLine("    <b>DLC:</b> " + vehicle.DlcName.ToLower());
                    }

                    gallery.WriteLine("  </div>");
                }

                gallery.WriteLine("</div>");
                gallery.WriteLine();
            }

            gallery.WriteLine("### Others");
            gallery.WriteLine("> [!NOTE]");
            gallery.WriteLine("> This vehicles are in the gamefiles, but can't be used.");
            gallery.WriteLine();
            gallery.WriteLine("<div class=\"grid-container\">");

            foreach (var vehicle in sortedVehiclesByNameSkipped)
            {
                gallery.WriteLine("  <div class=\"grid-item\">");

                var vehPath = imagePath + vehicle.Name.ToLower();

                gallery.WriteLine($"    <div class=\"grid-item-img\">");
                gallery.WriteLine($"      <img src=\"{vehPath}.png\" alt=\"Missing image &quot;{vehicle.Name}.png&quot;\" title=\"{vehicle.Name}\" loading=\"lazy\" />");
                gallery.WriteLine($"    </div>");

                if (vehicle.DlcName.ToLower() == "titleupdate")
                {
                    gallery.WriteLine("    <b>Name:</b> " + vehicle.Name + "<br/>");
                    gallery.WriteLine("    <b>Hash:</b> " + vehicle.HexHash + "<br/>");
                    gallery.WriteLine("    <b>Display Name:</b> " + vehicle.DisplayName );
                }
                else
                {
                    gallery.WriteLine("    <b>Name:</b> " + vehicle.Name + "<br/>");
                    gallery.WriteLine("    <b>Hash:</b> " + vehicle.HexHash + "<br/>");
                    gallery.WriteLine("    <b>Display Name:</b> " + vehicle.DisplayName + "<br/>");
                    gallery.WriteLine("    <b>DLC:</b> " + vehicle.DlcName.ToLower());
                }

                gallery.WriteLine("  </div>");
            }

            gallery.WriteLine("</div>");
            gallery.WriteLine();

            /*
             * Generate Snippets
             */
            gallery.WriteLine();
            gallery.WriteLine("## Snippets");
            gallery.WriteLine();

            gallery.WriteLine("# [JavaScript](#tab/tab1-0)");
            gallery.WriteLine("```js");
            gallery.WriteLine("export class VehicleModel {");

            foreach (var vehicle in sortedVehiclesByName)
            {

                gallery.WriteLine($"  static {vehicle.Name.ToLower()} = {vehicle.HexHash};");
            }

            gallery.WriteLine("}");
            gallery.WriteLine("```");
            gallery.WriteLine("# [TypeScript](#tab/tab1-1)");
            gallery.WriteLine("```ts");
            gallery.WriteLine("export enum VehicleModel {");

            foreach (var vehicle in sortedVehiclesByName)
            {
                gallery.WriteLine($"  {vehicle.Name.ToLower()} = {vehicle.HexHash};");
            }

            gallery.WriteLine("}");
            gallery.WriteLine("```");
            gallery.WriteLine("***");

            gallery.WriteLine();

            gallery.WriteLine("**Created with [GTA V Data Dumps from DurtyFree](https://github.com/DurtyFree/gta-v-data-dumps)**");

            gallery.Close();
            Console.WriteLine($"{vehiclesModelsFile} created for {sortedVehiclesByName.Count() + sortedVehiclesByNameSkipped.Count()} vehicles.");

            Console.WriteLine("This tool is using data files from https://github.com/DurtyFree/gta-v-data-dumps");
        }
    }
}
