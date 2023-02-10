using System;
using System.Net.Sockets;
using System.Text;


TcpClient tcpClient = new TcpClient();
tcpClient.Connect("127.0.0.1", 5000);
NetworkStream nStream = tcpClient.GetStream();
Console.WriteLine("Enter a message to be translated...");
string message = Console.ReadLine();
byte[] request = Serialize(message);
nStream.Write(request, 0, request.Length);
nStream.Flush();
// Read the first 4 bytes to determine the length of the response
byte[] lengthPrefix = new byte[4];
nStream.Read(lengthPrefix, 0, 4);
int length = BitConverter.ToInt32(lengthPrefix, 0);

// Read the response
byte[] response = new byte[length];
nStream.Read(response, 0, length);

// Deserialize the response
string translatedMessage = Encoding.ASCII.GetString(response);

// Display the translated message to the user
Console.WriteLine("Translated message: " + translatedMessage);


Console.ReadKey(); // Wait for keypress before exit
byte[] Serialize(string request)
{
    byte[] responseBytes = Encoding.ASCII.GetBytes(request);
    byte responseLength = (byte)responseBytes.Length;
    byte[] responseLengthBytes = BitConverter.GetBytes(responseLength);
    byte[] rawData = new byte[responseLength + 1];
    responseLengthBytes.CopyTo(rawData, 0);
    responseBytes.CopyTo(rawData, 1);
    return rawData;
}