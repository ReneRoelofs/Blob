using log4net.Core;

/// <summary>
/// Deze code, zonder namespace, kan worden opgenomen in een project om de textboxappender
/// in de LibForLog4Net te laten werken.
/// Het is me (nog) niet gelukt om dit zonder deze verwijzing te laten doen.
/// </summary>
public class TextBoxAppender : LibForLog4Net.TextBoxAppender
{
   

    protected override void Append(LoggingEvent[] loggingEvents)
    {
        base.Append(loggingEvents);
    }
  

}
