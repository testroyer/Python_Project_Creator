using System.Text.Json;
using Python_Project_Creator;


string origin_project_path = "";
string def_package_folder_name = "package";

//Create a json file for preferences


if (!File.Exists(@".\pref.json"))
{
    using (StreamWriter sw = new StreamWriter(File.Create(".\\pref.json")))
    {
        sw.Write("{\"PythonPath\" : \"C:\\\\\"}");
    }
}
var json = JsonSerializer.Deserialize<PathClass>(File.ReadAllText(".\\pref.json"));
string pyfolder = json!.PythonPath!;




if (args.Length > 0)
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
        using (StreamWriter sw = new StreamWriter(File.OpenWrite(".\\pref.json")))
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
}

ProjectName:
// {"PythonPath" : "C:\users\"}

var project_name = args.AsQueryable().FirstOrDefault();


Start:
if (project_name == null)
{
    Console.Write("Enter a project name:");
    project_name = Console.ReadLine()!.Trim();
}


string new_project_path = Path.Combine(pyfolder!, project_name);

if (Directory.Exists(new_project_path))
{
    Console.WriteLine("A project named like this already exists.");
    project_name = null;
    goto Start;
}

origin_project_path = new_project_path;
Directory.CreateDirectory(new_project_path);

async Task<int> CreateNewFile(string filename)
{
    try
    {
        using (StreamWriter fs = new StreamWriter(File.Create(Path.Combine(new_project_path, filename))))
        {
            if (filename == "data.json")
            {
                await fs.WriteAsync("{\n\n}");
            }
            if (filename == "main.py")
            {
                await fs.WriteAsync($"import json\nimport packages.{def_package_folder_name}\n\ndef main():\n    with open(\"./json /data.json\" , \"r\") as f:\n        data = json.load(f)\n\n\nif __name__ == '__main__':\n    main()");
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
    new_project_path = Path.Combine(origin_project_path, "json");
    Directory.CreateDirectory(new_project_path);
    await CreateNewFile("data.json");
    new_project_path = Path.Combine(origin_project_path, "packages");
    Directory.CreateDirectory(new_project_path);
    await CreateNewFile("__init__.py");
    await CreateNewFile((def_package_folder_name+".py"));


}
catch (Exception e)
{
    Console.WriteLine("Uh oh somethig went wrong:", e.Message);
    
}
finally
{
    Console.WriteLine("Program finish");
}

