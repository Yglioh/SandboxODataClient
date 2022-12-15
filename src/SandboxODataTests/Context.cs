using Microsoft.OData;
using Microsoft.OData.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sandbox.OData.Client;
using System.Diagnostics;
using Xunit.Abstractions;

namespace SandboxODataTests
{
    public class Context : Container, IDataServiceContextEvents
    {
        private static readonly Uri ServiceUri = new("https://localhost:44321");
        private const string BearerToken = @"5CgW6F7EZauAMKl90k4LbFeWQLRcxvAbZChWJIXDEg7N4zEv927NU7GQSxChOqs5KsMwOEK7MKy1R2wFGaQIEnnkvGlVg4lZwMHn4a45NZfdNB2h6sOrjO3VQcz/MQrUWT4JtKYHrFo=";

        private readonly ITestOutputHelper _output;

        public IODataRequestMessage? Request { get; private set; }
        public Descriptor? Descriptor { get; private set; }
        public string? Content { get; private set; }

        public bool Authenticate { get; set; }

        public Context(ITestOutputHelper output, bool authenticate = true) : base(ServiceUri)
        {
            _output = output;
            Authenticate = authenticate;
            MergeOption = MergeOption.NoTracking; // Needed to have nested expand properties filled in
            SendingRequest2 += OnSendingRequest;
        }

        private void OnSendingRequest(object? sender, SendingRequest2EventArgs e)
        {
            if (Authenticate)
                e.RequestMessage.SetHeader("Authorization", $"Bearer {BearerToken}");
            var message = e.RequestMessage;
            WriteLine($"HTTP {message.Method} {message.Url}");
            var descriptor = e.Descriptor as EntityDescriptor;
            if (descriptor != null)
            {
                WriteLine($"State = {descriptor.State}");
                WriteLine($"Entity = {descriptor.Entity}");
            }
            Request = e.RequestMessage;
            Descriptor = e.Descriptor;
        }

        void IDataServiceContextEvents.InspectContentStream(Stream stream)
        {
            if (stream == null || !stream.CanRead || !stream.CanSeek)
                return;
            var position = stream.Position;
            stream.Seek(0, SeekOrigin.Begin);
            using (var reader = new StreamReader(stream, leaveOpen: true))
            {
                var content = reader.ReadToEnd();
                var token = JToken.Parse(content);
                var json = token.ToString(Formatting.Indented);                
                WriteLine($"Content =\n{json}");
                Content = json;
#if DEBUG
                if (Debugger.IsAttached)
                    Debugger.Break();
#endif
            }
            stream.Seek(position, SeekOrigin.Begin);
        }

        private void WriteLine(string text)
        {
            _output?.WriteLine(text);
            Trace.WriteLine(text);
        }
    }
}
