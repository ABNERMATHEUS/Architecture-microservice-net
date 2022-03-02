using System;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using GeekShopping.CartAPI.Messages;
using GeekShopping.MessageBus;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace GeekShopping.CartAPI.RabbitMQSender;

public class RabbitMQMessageSender : IRabbitMQMessageSender
{
    private readonly string _hostName;
    private readonly string _password;
    private readonly string _userName;
    private IConnection _connection;

    private ILogger<RabbitMQMessageSender> _logger;

    public RabbitMQMessageSender(ILogger<RabbitMQMessageSender> logger)
    {
        _hostName = "localhost";
        _userName = "guest";
        _password = "guest";
        _logger = logger;
    }

    public void SendMessage(BaseMessage message, string queueName)
    {
        try
        {
            var factory = new ConnectionFactory();
            factory.HostName = _hostName;
            factory.UserName = _userName;
            factory.Password = _password;
            
            _connection = factory.CreateConnection();
            using var channel = _connection.CreateModel();

            channel.QueueDeclare(queue: queueName, false, false, false, arguments: null);
            var body = GetMessageAsByteArray(message);
            channel.BasicPublish(exchange: "", routingKey: queueName, basicProperties: null, body: body);
            _logger.LogInformation($"Publish success. Message: {message.ToString()}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,"Error RabbitMQ");
        }
    }

    private byte[] GetMessageAsByteArray(BaseMessage message)
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
        };
        var json = JsonSerializer.Serialize<CheckoutHeaderVO>((CheckoutHeaderVO) message, options);
        var body = Encoding.UTF8.GetBytes(json);
        return body;
    }
}