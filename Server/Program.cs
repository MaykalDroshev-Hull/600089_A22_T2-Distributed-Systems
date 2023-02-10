using System.Net;
using System.Net.Sockets;
using System.Text;

char[] consonants = new char[] { 'b', 'c', 'd', 'f', 'g', 'h', 'j', 'k', 'l', 'm', 'n', 'p', 'q', 'r', 's', 't', 'v', 'x', 'z', 'w', 'y' };
char[] vowels = new char[] { 'a', 'e', 'i', 'o', 'u' };
TcpListener tcpListener = new TcpListener(IPAddress.Parse("127.0.0.1"), 5000);
tcpListener.Start();
TcpClient tcpClient = tcpListener.AcceptTcpClient();
NetworkStream nStream = tcpClient.GetStream();
string message = ReadFromStream(nStream);
Console.WriteLine("Received: \"" + message + "\"");
string translatedMessage = Translate(message, consonants, vowels);

// Serialize the translated message
byte[] serializedMessage = Encoding.UTF8.GetBytes(translatedMessage);

// Create a length prefix for the serialized message
int length = serializedMessage.Length;
byte[] lengthPrefix = BitConverter.GetBytes(length);

// Write the length prefix and serialized message to the stream
nStream.Write(lengthPrefix, 0, lengthPrefix.Length);
nStream.Write(serializedMessage, 0, serializedMessage.Length);
nStream.Flush();

Console.WriteLine($"Written \"{translatedMessage}\"");
Console.ReadKey(); // Wait for keypress before exit



static string Translate(string message, char[] consonants, char[] vowels)
{
    string translatedmessage = string.Empty;
    string[] words = message.Split(' ');

    foreach (string word in words)
    {
        string tempWord = word;
        if (vowels.Any(e => tempWord.StartsWith(e)))
        {
            tempWord += "way";
        }
        else if (consonants.Any(e => tempWord.StartsWith(e) && consonants.Contains(tempWord[1])))
        {
            string firstLet = tempWord.Substring(0, 2);
            tempWord = tempWord.Remove(0, 2);
            tempWord = tempWord + firstLet + "ay";
        }
        else if (consonants.Any(e => tempWord.ToLower().StartsWith(e)))
        {
            string firstLet = tempWord.Substring(0, 1);
            tempWord = tempWord.Remove(0, 1);
            tempWord = tempWord + firstLet + "ay";
        }
        translatedmessage += tempWord + " ";
    }
    return translatedmessage.Trim();
}
static string ReadFromStream(NetworkStream stream)
{
    byte[] messageLengthBytes = new byte[1];
    stream.Read(messageLengthBytes, 0, 1);
    byte[] messageBytes = new byte[messageLengthBytes[0]];
    stream.Read(messageBytes, 0, messageLengthBytes[0]);
    return Encoding.ASCII.GetString(messageBytes);
}