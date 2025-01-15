namespace Paczkomat.consts;

public record Package(string Pin, string Phone, string Email, int Column, int Row, int Id)
{
    public SerializablePackage ToSerializable()
    {
        return new SerializablePackage
        {
            Id = Id,
            Column = Column,
            Email = Email,
            Phone = Phone,
            Pin = Pin,
            Row = Row,
        };
    }
    
}

public class SerializablePackage
{
    public string Pin = "";
    public string Phone = "";
    public string Email = "";
    public int Column = -1;
    public int Row = -1;
    public int Id = -1;

    public Package ToNormal() => new(Pin, Phone, Email, Column, Row, Id); 
}