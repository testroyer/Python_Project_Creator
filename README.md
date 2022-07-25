# Python_Project_Creator

Python Project Creator is a Command Line Tool developpped in C# within the goal of creating a python project through command line.

*Please note that this project won't be ready to use until v2.0.0*


Usage:

> ppc |projectname| -> Creates a project called |projectname| to the designated path. If a path wasn't specified by the user, it will create the project folder under the C:\  drive.

> ppc --projectpath -> Displays the designated path for creating projects.

> ppc --projectpath |projectpath| -> Sets the default project path to |projectpath|. This setting can be modified manuall from the pref.json file.

> ppc |projectname| -p |packagename| -> Sets the name of the package of the |projectname| to |packagename|.
