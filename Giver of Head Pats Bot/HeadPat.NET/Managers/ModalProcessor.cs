/*using System.Reflection;
using Discord.WebSocket;
using Serilog;

namespace HeadPats.Managers;
// Came from Private Repo: https://github.com/TotallyWholesome/TWNet/blob/master/TWNet/DiscordBot/Components/ModalProcessor.cs (Created by DDAkebono)
public class ModalProcessor {
    private static readonly ILogger Logger = Log.ForContext(typeof(ModalProcessor));
    private static Dictionary<string, ModalActionDelegate> _modalActions = null!;
    private delegate Task ModalActionDelegate(SocketModal modal);

    public ModalProcessor() {
        _modalActions = new Dictionary<string, ModalActionDelegate>();

        foreach (var method in typeof(ModalProcessor).GetMethods(BindingFlags.Static | BindingFlags.NonPublic)) {
            Logger.Debug($"Checking method {method.Name}");
            if (method.GetCustomAttribute(typeof(ModalAction)) is not ModalAction attr) continue;
            Logger.Debug($"Found method with attribute {attr.ModalTag}");
            
            _modalActions.Add(attr.ModalTag, (ModalActionDelegate)Delegate.CreateDelegate(typeof(ModalActionDelegate), null, method));
        }
        
        Logger.Information($"Modal Action Processor has loaded {_modalActions.Count} actions!");
    }
    
    public static async Task ProcessModal(SocketModal modal) {
        var modalId = modal.Data.CustomId.Split('-')[0].ToLower().Trim();
        if (!_modalActions.TryGetValue(modalId, out var value)) return;
        await value(modal);
    }
    
    #region Command Modals
    
    
    
    #endregion
}

public class ModalAction(string modalTag) : Attribute {
    public readonly string ModalTag = modalTag;
}*/