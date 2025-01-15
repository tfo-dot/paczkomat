using System.Xml.Serialization;
using Paczkomat.consts;

namespace Paczkomat.managers;

public class DataManager(string filePath)
{
    public void Serialize(DataContainer data)
    {
        var serializer = new XmlSerializer(typeof(DataContainer));
        using var writer = new StreamWriter(filePath);
        serializer.Serialize(writer, data);
    }

    public DataContainer Deserialize()
    {
        var serializer = new XmlSerializer(typeof(DataContainer));
        using var reader = new StreamReader(filePath);
        return (DataContainer)serializer.Deserialize(reader)!;
    }
}

[Serializable]
public class DataContainer
{
    public List<SerializablePackage> Packages = [];
    public List<string> Pins = [];

    public DataContainer AddPackages(List<Package> packages)
    {
        Packages.AddRange(packages.Select(elt => elt.ToSerializable()));

        return this;
    }
}