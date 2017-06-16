using UnityEngine;
using System.Collections;
using System;
using System.Diagnostics;
using System.Linq;

public static class InterProcessComunicationHelper
{
	/// <summary>
	/// Lance l'éxécutable spécifié au chemin de "processPath" et lui envoie les arguments spécifié dans "processArguments".
	/// Example : LaunchProcess(@"C:\Users\Ben\Desktop\executable.exe", new string[] { "firstParam", "secondParam" });
	/// </summary>
	public static Process LaunchProcess(
		string processPath,
		string[] processArguments)
	{
		string processArgument = "";

		// On a besoin d'entouré chaque argument de quote pour qu'il soit reconnu comme argument unique si il contiend des espaces.
		for (int i = 0; i < processArguments.Length; i++)
			processArgument += "\"" + processArguments[i] + "\" ";

		Process process = new Process();

		process.StartInfo.Arguments = processArgument;
		process.StartInfo.FileName = processPath;
		// Permet de cacher la console du processu avec qui l'on communique.
		//process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
		
		process.Start();

		return process;
	}

	/// <summary>
	/// Lance une commande SSH en C#.
	/// Pour que cette commande fonctionne, il faut rajouter le binaire ssh "C:/program files/git/usr/bin/ssh" dans les variables d'environnement "PATH".
	/// Puis redémarrer le PC.
	/// </summary>
	public static Process LaunchSSHCommand(
		string command = "echo t",
		string computerIP = "192.168.1.200",
		string computerUsername = "vincentberlioz",
		string computerPassword = "0000")
	{
		Process process = new Process();

		process.StartInfo.FileName = "ssh";
		process.StartInfo.Arguments = string.Format(
			"{0}@{1} {2}",
			computerUsername,
			computerIP,
			command);

		// Affiche le résultat de la console ssh dans cette console.
		process.StartInfo.UseShellExecute = false;

		process.Start();

		// Affiche l'output de la commande taper.
		//UnityEngine.Debug.Log(process.StandardOutput.ReadToEnd());

		return process;
	}
}