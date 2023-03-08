using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using System.IO;

namespace ConsoleApp1
{
  class txtfileSearch
  {
    public void Search()
    {

    }
  }

  [Serializable]
  class txtFile
  {
    public string text;
    public string tags;
    public txtFile (string text, string tags)
    {
      this.text = text;
      this.tags = tags;
    }

    public void Serialize(FileStream fs)
    {
      BinaryFormatter bf = new BinaryFormatter();
      bf.Serialize(fs, this);
      fs.Flush();
      fs.Close();
    }

    public void Deserialize(FileStream fs)
    {
      BinaryFormatter bf = new BinaryFormatter();
      txtFile deserialized = (txtFile)bf.Deserialize(fs);
      text = deserialized.text;
      fs.Close();
    }

    public void PrintText()
    {
      Console.WriteLine(text);
    }
  }
  class Program
  {
    static void Main(string[] args)
    {
      FileStream fs = new FileStream("people.dat", FileMode.OpenOrCreate, FileAccess.Write);

      string s = "работает !";
      string tags1 = "какой то тег";
      txtFile t = new txtFile(s, tags1);
      t.PrintText();
      t.Serialize(fs);

      string s2 = "gfowie89rfu34h-9f84yf-04";
      t = new txtFile(s2, tags1);
      t.PrintText();

      fs = new FileStream("people.dat", FileMode.OpenOrCreate, FileAccess.Read);
      t.Deserialize(fs);
      t.PrintText();

      Console.ReadKey();
    }
  }
}
