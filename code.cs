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
    public string Text { get; set; }
  }
  public interface IOriginator
  {
    object GetMemento();
    void SetMemento(object memento);
  }

  [Serializable]
  public class TxtFile : IOriginator
  {
    public string Text;
    public string Tags;

    public TxtFile() { }

    public TxtFile(string Text, string Tags)
    {
      this.Text = Text;
      this.Tags = Tags;
    }

    public string BinarySerialize()
    {
      string FileName = "File.dat";
      FileStream fileStream = new FileStream(FileName, FileMode.OpenOrCreate, FileAccess.Write);
      BinaryFormatter binaryFormat = new BinaryFormatter();
      binaryFormat.Serialize(fileStream, this);
      fileStream.Flush();
      fileStream.Close();
      return FileName;
    }

    public void BinaryDeserialize(string FileName)
    {
      FileStream fileStream = new FileStream(FileName, FileMode.OpenOrCreate, FileAccess.Read);
      BinaryFormatter binaryFormat = new BinaryFormatter();
      TxtFile deserialized = (TxtFile)binaryFormat.Deserialize(fileStream);
      Text = deserialized.Text;
      fileStream.Close();
    }

    static public string XMLSerialize(TxtFile Details)
    {
      string FileName = "File.xml";
      XmlSerializer serializer = new XmlSerializer(typeof(TxtFile));
      using (TextWriter Writer = new StreamWriter(FileName))
      {
        serializer.Serialize(Writer, Details);
      }
      return FileName;
    }

    static public TxtFile XMLDeserialize(string FileName)
    {
      XmlSerializer deserializer = new XmlSerializer(typeof(TxtFile));
      TextReader reader = new StreamReader(FileName);
      object obj = deserializer.Deserialize(reader);
      TxtFile XmlData = (TxtFile)obj;
      reader.Close();
      return XmlData;
    }

    public void PrintText()
    {
      Console.WriteLine(Text);
    }

    object IOriginator.GetMemento()
    {
      return new Memento { Text = this.Text };
    }
    void IOriginator.SetMemento(object memento)
    {
      if (memento is Memento)
      {
        Text = (memento as Memento).Text;
      }
    }
  }
  public class Caretaker
  {
    private object memento;
    public void SaveState(IOriginator Originator)
    {
      memento = Originator.GetMemento();
    }

    public void RestoreState(IOriginator Originator)
    {
      Originator.SetMemento(memento);
    }
  }

  class FileSearch
  {
    public string FoundFiles = "";
    public void Search(TxtFile[] Library, string Request, int NumberOfFiles)
    {
      for (int FileNumber = 0; FileNumber < NumberOfFiles; ++FileNumber)
      {
        if (Library[FileNumber].Tags == Request)
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
      TxtFile[] Library = new TxtFile[NumberOfFiles];
      TxtFile File;

      File = new TxtFile("какой то текст первого файла", "тег3");
      Library[0] = File;
      File = new TxtFile("какой то текст второго файла", "тег8");
      Library[1] = File;
      File = new TxtFile("какой то текст третьего файла", "тег3");
      Library[2] = File;
      File = new TxtFile("какой то текст четвертого файла", "тег5");
      Library[3] = File;
      File = new TxtFile("какой то текст пятого файла", "тег1");
      Library[4] = File;

      Console.WriteLine("Файл с каким тегом необходим: ");
      string Request = Convert.ToString(Console.ReadLine());
        
      FileSearch FileSearch = new FileSearch();
      FileSearch.Search(Library, Request, NumberOfFiles);
      Console.WriteLine(FileSearch.FoundFiles);

      Console.WriteLine("Выберите файл, который необходимо отредактировать: ");
      int FileNumber = Convert.ToInt32(Console.ReadLine());

      string FileName = TxtFile.XMLSerialize(Library[FileNumber]);

      Console.WriteLine("текст этого файла:");
      Caretaker Caretaker = new Caretaker();
      Library[FileNumber].PrintText();
      Caretaker.SaveState(Library[FileNumber]);

      Console.WriteLine("Введите новый текст файла: ");
      string NewText = Convert.ToString(Console.ReadLine());
      Library[FileNumber].Text = NewText;
      FileName = TxtFile.XMLSerialize(Library[FileNumber]);

      Console.WriteLine("Файл сохранен и стал:");
      Library[FileNumber].PrintText();

      Console.WriteLine("Отменить изменения ?(да/нет)");
      string SaveChoice = Convert.ToString(Console.ReadLine());

      if (SaveChoice == "да")
      {
        Caretaker.RestoreState(Library[FileNumber]);
        FileName = TxtFile.XMLSerialize(Library[FileNumber]);
        Console.WriteLine("Файл остался прежним: ");
        Library[FileNumber].PrintText();
      }

      Console.ReadKey();
    }
  }
}
