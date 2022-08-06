using System.Text.Json;
using Python_Project_Creator;


string origin_project_path = "";
string def_package_folder_name = "package";
string def_json_name = "data.json";
bool isBlank = false;
bool noJson = false;
bool noPack;

//Create a json file for preferences
string currrent_directory = Directory.GetCurrentDirectory();
string[] current_directory_array = currrent_directory.Split('\\');
string current_user_directory = current_directory_array[2];

if (!File.Exists(@$"C:\Users\{current_user_directory}\AppData\Roaming\ppc\pref.json"))
{
    Directory.CreateDirectory(@$"C:\Users\{current_user_directory}\AppData\Roaming\ppc");
    using (StreamWriter sw = new StreamWriter(File.Create(@$"C:\Users\{current_user_directory}\AppData\Roaming\ppc\pref.json")))
    {
        sw.Write("{\"PythonPath\" : \"C:\\\\\"}");
    }
}
var json = JsonSerializer.Deserialize<PathClass>(File.ReadAllText(@$"C:\Users\{current_user_directory}\AppData\Roaming\ppc\pref.json"));
string pyfolder = json!.PythonPath!;

void GetHelp()
{
    Console.Write(@"PPC version 1.8.2

Usage:

 - ppc --version -> Prints out the version.

 - ppc <projectname> -> Creates a project called <projectname> to the designated path. (Flag modifiable)

 - ppc --projectpath -> Prints out the designated file path. If a path hasn't been specified before then the designated path will be C:\\

 - ppc --projectpath <projectpath> -> Sets the designated file path to <projectpath>. 

 - ppc -p <packagename> -> The package in the project will be named <packagename>.

 - ppc -json <jsonname> -> The json file in the project will be named <jsonname>.

 - ppc --blank -> The project which will be created will just be a blank main.py. If this is used with the 'and' function a true or false value must be determined.

 - ppc --nojson -> The prject witch will be created won't have a json file. If this is used with the 'and' function a true or false value must be determined


Note: You can use the 'and' keyword to combine the --projectpath, --blank (true or false), --nojson (true or false), -p and -json flags.
");
    Environment.Exit(0);
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
            }

            goto ProjectName;
        }



        if (args.Length == 1 && args[0] == "--projectpath")
        {

            Console.WriteLine($"Project path: {pyfolder}");
            Environment.Exit(0);
        }
        else if (args.Length == 2 && args[0] == "--projectpath")
        {
            json!.PythonPath = args[1];
            string jsonString = JsonSerializer.Serialize(json);
            using (StreamWriter sw = new StreamWriter(File.OpenWrite(@$"C:\Users\{current_user_directory}\AppData\Roaming\ppc\pref.json")))
            {
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
            Console.WriteLine($"The version 1.8.2 of ppc is currently installed"); // Litterally the wors possible solution to the problem.
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
            if (filename == "main.py" && isBlank == false && noJson == false)
            {
                await fs.WriteAsync($"import json\nimport os\nimport math\nimport sys\nimport time\nimport packages.{def_package_folder_name}\n\ndef main():\n    with open(\"./json/{def_json_name}\" , \"r\") as f:\n        data = json.load(f)\n\n\nif __name__ == '__main__':\n    main()");
            }
            else if (filename == "main.py" && isBlank == true)
            {
                await fs.WriteAsync("");
            }
            else if (filename == "main.py" && noJson == true)
            {
                await fs.WriteAsync($"import json\nimport os\nimport math\nimport sys\nimport time\nimport packages.{def_package_folder_name}\n\ndef main():\n    pass\n\n\nif __name__ == '__main__':\n    main()");
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
        


}
catch (Exception e)
{
    Console.WriteLine("Uh oh somethig went wrong:", e.Message);
    
}
finally
{
    Console.WriteLine("Program finish");
}

