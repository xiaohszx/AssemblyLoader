﻿// *****************************************************************************
// File:       Program.cs
// Solution:   AssemblyLoader
// Project:    Tests
// Date:       11/25/2018
// Author:     Latency McLaughlin
// Copywrite:  Bio-Hazard Industries - 1998-2018
// ***************************************************************************** 

using System;
using System.IO;
using System.Reflection;
using AssemblyLoader;
using NUnit.Framework;

namespace Tests {
  public class Program {
    [Test]
    public void Load() {
      // Dynamically find the embedded resource assembly included within the project to test against.
      var assembly = Assembly.GetExecutingAssembly();
      foreach (var s in assembly.GetManifestResourceNames())
        using (var stream = assembly.GetManifestResourceStream(s)) {
          if (stream != null) {
            // Test overload +1 - Stream
            var asm = AssemblyLoader.Load.Assembly(stream);
            Console.WriteLine("Stream Test:  " + (!string.IsNullOrEmpty(asm?.FullName) ? "Success" : "Fail"));

            var data = new byte[stream.Length];
            stream.Seek(0, SeekOrigin.Begin);
            stream.Read(data, 0, data.Length);
            // Test overload +2 - byte[]
            asm = AssemblyLoader.Load.Assembly(data);
            Console.WriteLine("Byte[] Test:  " + (!string.IsNullOrEmpty(asm?.FullName) ? "Success" : "Fail"));

            // These next one should always be null, since there are no embedded assemblies within AssemblyInfo or contains the name like that found in Tests. 
            // Since we are running these with VST in a proxy rather than assemblies loaded in the executing assembly, loaded assemblies can not be unloaded
            // without bringing down the domain.   There must be a host domain to call our methods.
            //
            // I will leaving this one in for backwards compatibility, which will mean that any local reference assembly will need to be converted from <string>
            // to <AssemblyName> in order to be used and overload the <string> calling convention.   This should always fail as no dependencies exist.

            // Test overload +3 - AssemblyName
            asm = AssemblyLoader.Load.Assembly(asm.GetName());
            Assert.Null(asm);

            // The next test will require that we dump the embedded resource assembly into $TEMP and read it back in as a byte stream which will reuse
            // code blocks already defined for <byte[]>.

            var fileName = Path.GetTempPath() + s.Substring(s.IndexOf('.') + 1);
            if (File.Exists(fileName)) // Overwrite if desired  (depending on your needs)
              File.Delete(fileName);
            File.WriteAllBytes(fileName, data);

            // Test overload +4 - string
            asm = AssemblyLoader.Load.Assembly(fileName);
            Console.WriteLine("FileName Test:  " + (!string.IsNullOrEmpty(asm?.FullName) ? "Success" : "Fail"));
          }
        }
    }


    [Test]
    public void Extensions() {
      var asm = Assembly.GetAssembly(typeof(Extensions));

      Console.WriteLine($"ProductTitle:  {asm.ProductTitle()}");
      Console.WriteLine($"AssemblyVersion:  {asm.AssemblyVersion()}");
      Console.WriteLine($"AssemblyFileVersion:  {asm.AssemblyFileVersion()}");
      Console.WriteLine($"Description:  {asm.Description()}");
      Console.WriteLine($"Product:  {asm.Product()}");
      Console.WriteLine($"Copyright:  {asm.Copyright()}");
      Console.WriteLine($"Company:  {asm.Company()}");
      Console.WriteLine($"Trademark:  {asm.Trademark()}");
    }
  }
}