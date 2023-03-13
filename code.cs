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
  class Memento
  {
    public string text { get; set; }
  }
  public interface IOriginator
  {
    object GetMemento();
    void SetMemento(object memento);
  }

  [Serializable]
  public class txtFile : IOriginator
  {
    public string text;
    public string tags;

    public txtFile() { }

    public txtFile(string text, string tags)
    {
      this.text = text;
      this.tags = tags;
    }

    public string BinarySerialize()
    {
      string FileName = "file.dat";
      FileStream fs = new FileStream(FileName, FileMode.OpenOrCreate, FileAccess.Write);
      BinaryFormatter bf = new BinaryFormatter();
      bf.Serialize(fs, this);
      fs.Flush();
      fs.Close();
      return FileName;
    }

    public void BinaryDeserialize(string FileName)
    {
      FileStream fs = new FileStream(FileName, FileMode.OpenOrCreate, FileAccess.Read);
      BinaryFormatter bf = new BinaryFormatter();
      txtFile deserialized = (txtFile)bf.Deserialize(fs);
      text = deserialized.text;
      fs.Close();
    }

    static public string XMLSerialize(txtFile details)
    {
      string FileName = "file.xml";
      XmlSerializer serializer = new XmlSerializer(typeof(txtFile));
      using (TextWriter writer = new StreamWriter(FileName))
      {
        serializer.Serialize(writer, details);
      }
      return FileName;
    }

    static public txtFile XMLDeserialize(string FileName)
    {
      XmlSerializer deserializer = new XmlSerializer(typeof(txtFile));
      TextReader reader = new StreamReader(FileName);
      object obj = deserializer.Deserialize(reader);
      txtFile XmlData = (txtFile)obj;
      reader.Close();
      return XmlData;
    }

    public void PrintText()
    {
      Console.WriteLine(text);
    }

    object IOriginator.GetMemento()
    {
      return new Memento { text = this.text };
    }
    void IOriginator.SetMemento(object memento)
    {
      if (memento is Memento)
      {
        var mem = memento as Memento;
        text = mem.text;
      }
    }
  }
  public class Caretaker
  {
    private object memento;
    public void SaveState(IOriginator originator)
    {
      memento = originator.GetMemento();
    }

    public void RestoreState(IOriginator originator)
    {
      originator.SetMemento(memento);
    }
  }

  class FileSearch
  {
    public string FoundFiles = "";
    public void Search(txtFile[] library, string Request, int numberOfFiles)
    {
      for (int FileNumber = 0; FileNumber < numberOfFiles; ++FileNumber)
      {
        if (library[FileNumber].tags == Request)
        {
          FoundFiles += FileNumber + " ";
        }
      }

      if (FoundFiles == "")
      {
        Console.WriteLine("файл с таким тегом не найден");
      }
      else
      {
        Console.WriteLine("найденные файлы с нужным тегом: ");
      }
    }
  }

  class Program
  {
    static void Main(string[] args)
    {

      const int NumberOfFiles = 5;
      txtFile[] Library = new txtFile[NumberOfFiles];
      txtFile file;

      file = new txtFile("какой то текст первого файла", "тег3");
      Library[0] = file;
      file = new txtFile("какой то текст второго файла", "тег8");
      Library[1] = file;
      file = new txtFile("какой то текст третьего файла", "тег3");
      Library[2] = file;
      file = new txtFile("какой то текст четвертого файла", "тег5");
      Library[3] = file;
      file = new txtFile("какой то текст пятого файла", "тег1");
      Library[4] = file;

      Console.WriteLine("файл с каким тегом необходим: ");
      string Request = Convert.ToString(Console.ReadLine());

      FileSearch filesearch = new FileSearch();
      filesearch.Search(Library, Request, NumberOfFiles);
      Console.WriteLine(filesearch.FoundFiles);

      Console.WriteLine("выберите файл, который необходимо отредактировать: ");
      int FileNumber = Convert.ToInt32(Console.ReadLine());

      string FileName = txtFile.XMLSerialize(Library[FileNumber]);

      Console.WriteLine("текст этого файла:");
      Caretaker ct = new Caretaker();
      Library[FileNumber].PrintText();
      ct.SaveState(Library[FileNumber]);

      Console.WriteLine("Введите новый текст файла: ");
      string NewText = Convert.ToString(Console.ReadLine());
      Library[FileNumber].text = NewText;
      FileName = txtFile.XMLSerialize(Library[FileNumber]);

      Console.WriteLine("сохранить ?(да/нет)");
      string SaveChoice = Convert.ToString(Console.ReadLine());

      if (SaveChoice == "нет")
      {
        ct.RestoreState(Library[FileNumber]);
        FileName = txtFile.XMLSerialize(Library[FileNumber]);
        Console.WriteLine("файл остался прежним: ");
        Library[FileNumber].PrintText();
      }
      else
      {
        Console.WriteLine("файл сохранен и стал:");
        Library[FileNumber].PrintText();
      }

      Console.ReadKey();
    }
  }
}
