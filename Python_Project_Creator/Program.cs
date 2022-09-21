using System.Text.Json;
using Python_Project_Creator;
using System.Diagnostics;


string version = "1.9.7";

string origin_project_path = "";
string def_package_folder_name = "package";
string def_json_name = "data.json";
bool isBlank = false;
bool noJson = false;
bool noPack = false;

//Create a json file for preferences
string currrent_directory = Directory.GetCurrentDirectory();
string[] current_directory_array = currrent_directory.Split('\\');
string current_user_directory = current_directory_array[2];

if (!File.Exists(@$"C:\Users\{current_user_directory}\AppData\Roaming\ppc\pref.json")) // Somwhere in the code it adds }" to and of the json file
{
    Directory.CreateDirectory(@$"C:\Users\{current_user_directory}\AppData\Roaming\ppc");
    using (StreamWriter sw = new StreamWriter(File.Create(@$"C:\Users\{current_user_directory}\AppData\Roaming\ppc\pref.json")))
    {
        sw.Write("{\"PythonPath\" : \"C:\\\\\"}");
    }
}

if (!Directory.Exists(@$"C:\Users\{current_user_directory}\AppData\Roaming\ppc\presets"))
{
    Directory.CreateDirectory(@$"C:\Users\{current_user_directory}\AppData\Roaming\ppc\presets");
}


var json = JsonSerializer.Deserialize<PathClass>(File.ReadAllText(@$"C:\Users\{current_user_directory}\AppData\Roaming\ppc\pref.json"));
string pyfolder = json!.PythonPath!;

void GetHelp()
{
    Console.Write(@$"PPC version {version}

Usage:

 - ppc --version -> Prints out the version.

 - ppc <projectname> -> Creates a project called <projectname> to the designated path. (Flag modifiable)

 - ppc --projectpath -> Prints out the designated file path. If a path hasn't been specified before then the designated path will be C:\\

 - ppc --projectpath <projectpath> -> Sets the designated file path to <projectpath>. 

 - ppc -p <packagename> -> The package in the project will be named <packagename>.

 - ppc -json <jsonname> -> The json file in the project will be named <jsonname>.

 - ppc --blank -> The project which will be created will just be a blank main.py. If this is used with the 'and' function a true or false value must be determined.

 - ppc --nojson -> The prject witch will be created won't have a json file. If this is used with the 'and' function a true or false value must be determined

 - ppc --nopack -> The prject witch will be created won't have a pre made package. If this is used with the 'and' function a true or false value must be determined

 - ppc --create-preset <presetpath> <presetname> -> Creates a project preset for later used named <projectname> under th ppc folder.

 - ppc --presets -> Displays the premade presets.
 
 - ppc --delete-preset <presetname> -> Deletes preset named <presetname>.

Note: You can use the 'and' keyword to combine the --projectpath, --blank (true or false), --nojson (true or false), --nopack (true or false), -p and -json flags.
");
    Environment.Exit(0);
}

void CopyDirectory(string directory , string target)
{
    Directory.CreateDirectory(target);

    //Get files ad copy them to the target
    string[] files = Directory.GetFiles(directory);
    foreach (string file in files)
    {
        string[] file_array = file.Split("\\");
        File.Copy(file, Path.Combine(target , file_array[file_array.Length - 1]));
    }

    //Get directoriesstring
    string[] dirs = Directory.GetDirectories(directory);
    foreach (string dir in dirs)
    {
        string[] directory_array = dir.Split("\\");
        CopyDirectory(Path.Combine(directory, directory_array[directory_array.Length - 1]), Path.Combine(target, directory_array[directory_array.Length - 1]));
    }

}

if (args.Length > 0)
{
    try
    {
        List<string> arr = args.ToList();
        if (arr.Contains("and"))
        {
            int left = 1;
            int right = 1;
            List<List<string>> comandarg = new List<List<string>>();
            while (right < arr.Count)
            {
                if (right == arr.Count - 1)
                    comandarg.Add(arr.Skip(left).Take((right + 1) - left).ToList());
                if (arr[right] == "and")
                {
                    comandarg.Add(arr.Skip(left).Take(right - left).ToList());
                    left = right;
                    left++;
                    right++;
                    continue;
                }
                right++;
            }
            foreach (List<string> commands in comandarg)
            {
                if (commands[0] == "--projectpath")
                {
                    pyfolder = commands[1];
                }
                else if (commands[0] == "-p")
                {
                    def_package_folder_name = commands[1];  
                }
                else if (commands[0] == "-json")
                {
                    if (commands[1].EndsWith(".json"))
                    {
                        def_json_name = commands[1];
                    }
                    else
                    {
                        def_json_name = commands[1] + ".json";
                    }
                }else if (commands[0] == "--blank")
                {
                    if (commands[1].ToLower() == "true")
                    {
                        isBlank = true;
                    }
                    else if (commands[1].ToLower() == "false")
                    {
                        isBlank = false;
                    }
                }
                else if (commands[0] == "--nojson")
                {
                    if (commands[1].ToLower() == "true")
                    {
                        noJson = true;
                    }
                    else if (commands[1].ToLower() == "false")
                    {
                        noJson = false;
                    }
                }
                else if (commands[0] == "--nopack")
                {
                    if (commands[1].ToLower() == "true")
                    {
                        noPack = true;
                    }
                    else if (commands[1].ToLower() == "false")
                    {
                        noPack = false;
                    }
                }
            }

            goto ProjectName;
        }


        if (args.Length == 1 && args[0] == "--projectpath")
        {

            Console.WriteLine($"Project path: {pyfolder}");
            Environment.Exit(0);
        }
        else if (args.Length == 3 && args[0] == "--create-preset")
        {
            string directory_arg = args[1];
            string target_arg = args[2];
            string target_path = Path.Combine(@$"C:\Users\{current_user_directory}\AppData\Roaming\ppc\presets", target_arg);
            if (Directory.Exists(target_path))
            {
                Console.WriteLine("A preset like that already exists.");
                Environment.Exit(0);
            
            }

            if (directory_arg.StartsWith("."))
            {
                directory_arg = directory_arg.Substring(2);
                CopyDirectory(Path.Combine(current_directory_array[current_directory_array.Length - 1] , directory_arg), Path.Combine(@$"C:\Users\{current_user_directory}\AppData\Roaming\ppc\presets" , target_arg));
            }
            else
            {
                CopyDirectory(Path.Combine(current_directory_array[current_directory_array.Length - 1] , directory_arg), Path.Combine(@$"C:\Users\{current_user_directory}\AppData\Roaming\ppc\presets" , target_arg));
            }

            Console.WriteLine($"The template {target_arg} has been created successfully");
            Environment.Exit(0);

        }
        else if (args.Length == 1 && args[0] == "--presets")
        {
            string[] presets = Directory.GetDirectories(@$"C:\Users\{current_user_directory}\AppData\Roaming\ppc\presets");
            if (presets.Length != 0)
            {
                Console.WriteLine("Avalible presets are:");
                foreach (string preset in presets)
                {
                    //string[] preset_arr = preset.Split("\\");
                    Console.WriteLine($"> {Path.GetFileName(preset)}");
                }
            }
            else
            {
                Console.WriteLine("There aren't any presets avalible");
            }
            Environment.Exit(0);
        }
        else if (args.Length == 2 && args[0] == "--delte-preset")
        {

            string target_preset = args[1];
            Get_Confirm:
            Console.Write($"Are you sure you want to delete '{target_preset}', this cant be reverted (y/n): ");
            string? confirm = Console.ReadLine();
            if (confirm == "y" || confirm == "Y")
            {
                string target_presed_folder = Path.Combine(@$"C:\Users\{current_user_directory}\AppData\Roaming\ppc\presets", target_preset);
                if (Directory.Exists(target_presed_folder))
                {
                    Directory.Delete(target_presed_folder , true);
                    Console.WriteLine($"Preset '{target_preset}' was removed succesfully.");
                    Environment.Exit(0);
                }
                else
                {
                    Console.WriteLine($"There isn't any preset named {target_preset}");
                    Environment.Exit(1);    
                }
            }
            else if (confirm == "n" || confirm == "N")
            {
                Console.WriteLine("Deletion aborted.");
                Environment.Exit(0);
            }
            else
            {
                Console.WriteLine("You should enter an accaptable value");
                goto Get_Confirm;
            }


        }
        else if (args.Length == 2 && args[0] == "--projectpath") // Somwhere in the code it adds }" to and of the json file
        {
            json!.PythonPath = args[1];
            string jsonString = JsonSerializer.Serialize(json);
            File.WriteAllText(@$"C:\Users\{current_user_directory}\AppData\Roaming\ppc\pref.json", String.Empty); // Empities string before writing to prevent issues caused by length.
            using (StreamWriter sw = new StreamWriter(File.OpenWrite(@$"C:\Users\{current_user_directory}\AppData\Roaming\ppc\pref.json")))
            {
                sw.Write("");
                sw.Write(jsonString);
            }
            Console.WriteLine($"New path changed to: {json.PythonPath}");
            Environment.Exit(0);

        }
        else if (args.Length == 3 && args[1] == "--projectpath")
        {
            pyfolder = args[2];
        }
        else if (args.Length == 3 && args[1] == "-p")
        {
            def_package_folder_name = args[2];
        }
        else if (args.Length == 3 && args[1] == "-json")
        {
            if (args[1].EndsWith(".json"))
            {
                def_json_name = args[2];
            }
            else
            {
                def_json_name = args[2] + ".json";
            }
        }
        else if (args.Length == 1 && args[0] == "--version")
        {
            Console.WriteLine($"The version {version} of ppc is currently installed"); // Litterally the wors possible solution to the problem.
            Environment.Exit(0);
        }
        else if (args.Length == 1 && (args[0] == "--help" || args[0] == "-h"))
        {
            GetHelp();
        }
        else if (args.Length == 2 && args[1] == "--blank")
        {
            isBlank = true;
        }
        else if (args.Length == 2 && args[1] == "--nojson")
        {
            noJson = true;
        }
        else if (args.Length == 2 && args[1] == "--nopack")
        {
            noPack = true;
        }
    }
    catch (Exception e)
    {
        Console.WriteLine(e.Message);
        Environment.Exit(1);
    }
}

ProjectName:
// {"PythonPath" : "C:\users\"}

var project_name = args.AsQueryable().FirstOrDefault();


Start: 
if (project_name == null)
{
    GetHelp();
    Environment.Exit(0);
}


string new_project_path = Path.Combine(pyfolder!, project_name);

if (Directory.Exists(new_project_path)) // Application exit is nescessary or could do an infinite loop.
{
    Console.WriteLine("A project named like this already exists.");
    Environment.Exit(0);
}

origin_project_path = new_project_path;
Directory.CreateDirectory(new_project_path);

async Task<int> CreateNewFile(string filename)
{
    try
    {
        using (StreamWriter fs = new StreamWriter(File.Create(Path.Combine(new_project_path, filename))))
        {
            if (filename == def_json_name)
            {
                await fs.WriteAsync("{\n\n}");
            }
            if (filename == "main.py" && isBlank == false && noJson == false && noPack == false)
            {
                await fs.WriteAsync($"import json\nimport os\nimport math\nimport sys\nimport time\nimport packages.{def_package_folder_name}\n\ndef main():\n    with open(\"./json/{def_json_name}\" , \"r\") as f:\n        data = json.load(f)\n\n\nif __name__ == '__main__':\n    main()");
            }
            else if (filename == "main.py" && isBlank == true)
            {
                await fs.WriteAsync("");
            }
            else if (filename == "main.py" && noJson == true && noPack == false)
            {
                await fs.WriteAsync($"import json\nimport os\nimport math\nimport sys\nimport time\nimport packages.{def_package_folder_name}\n\ndef main():\n    pass\n\n\nif __name__ == '__main__':\n    main()");
            }
            else if (filename == "main.py" && noJson == false && noPack == true)
            {
                await fs.WriteAsync($"import json\nimport os\nimport math\nimport sys\nimport time\n\n\ndef main():\n    with open(\"./json/{def_json_name}\" , \"r\") as f:\n        data = json.load(f)\n\n\nif __name__ == '__main__':\n    main()");
            }
            else if (filename == "main.py" && noJson == true && noPack == true)
            {
                await fs.WriteAsync($"import json\nimport os\nimport math\nimport sys\nimport time\n\n\ndef main():\n    pass\n\n\nif __name__ == '__main__':\n    main()");
            }
        }
    }
    catch (DirectoryNotFoundException)
    {
        Console.WriteLine("Oops could not found the directory looking for");
        await CreateNewFile(filename!);
    }
    return 0;
}

try
{

    await CreateNewFile("main.py");
    if (!isBlank)
    {
        if (!noJson)
        {
            //Creation of the json directory and files
            new_project_path = Path.Combine(origin_project_path, "json");
            Directory.CreateDirectory(new_project_path);
            await CreateNewFile(def_json_name);
        }
        
        if (!noPack) {
            // Creation of the packages folder and files
            new_project_path = Path.Combine(origin_project_path, "packages");
            Directory.CreateDirectory(new_project_path);
            await CreateNewFile("__init__.py");
            await CreateNewFile((def_package_folder_name + ".py"));
        }
        
    }

    string command = @$"/C cd {origin_project_path} && code .";
    Process.Start("cmd.exe", command);

}
catch (Exception e)
{
    Console.WriteLine("Uh oh somethig went wrong:", e.Message);
    
}
finally
{
    Console.WriteLine("Program finished");
}

