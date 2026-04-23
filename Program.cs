using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

class Program
{
    private DiscordSocketClient _client;
    private Random rand = new Random();

    private readonly ulong[] AdminRoles =
    {
        1481765988147138772,
        1496291548936142919,
        1477658738969284699
    };

    private readonly ulong OWNER_ID = 1218638987208691858;
    private readonly ulong other = 1246224666083725465;
    private readonly ulong lul = 939233522504327169;

    public static Task Main(string[] args)
        => new Program().MainAsync();

    public async Task MainAsync()
    {
        _client = new DiscordSocketClient(new DiscordSocketConfig
        {
            GatewayIntents =
                GatewayIntents.Guilds |
                GatewayIntents.GuildMessages |
                GatewayIntents.MessageContent |
                GatewayIntents.GuildMembers
        });

        _client.Log += Log;
        _client.MessageReceived += MessageReceived;

        await _client.LoginAsync(TokenType.Bot, "MTQ5NjI4OTQ0NjE2NzgzOTAwMw.GdKAVl.4oZETtycEFuhvZ1WmB6SUoQTEwMgJJsF_Bl5X8");
        await _client.StartAsync();

        Console.WriteLine("Bot running...");
        await Task.Delay(-1);
    }

    private Task Log(LogMessage msg)
    {
        Console.WriteLine(msg);
        return Task.CompletedTask;
    }

    // =========================
    // MESSAGE HANDLER
    // =========================
    private async Task MessageReceived(SocketMessage message)
    {
        if (message.Author.IsBot) return;

        string msg = message.Content.ToLower();
        var user = message.Author;

        // =========================
        // BASIC COMMANDS
        // =========================
        if (msg == "?ping") await message.Channel.SendMessageAsync("🏓 Pong!");
        else if (msg == "?hello") await message.Channel.SendMessageAsync($"👋 Hello {user.Mention}");
        else if (msg == "?bye") await message.Channel.SendMessageAsync("👋 Goodbye!");
        else if (msg == "?joke") await message.Channel.SendMessageAsync(GetJoke());
        else if (msg == "?quote") await message.Channel.SendMessageAsync("💡 Keep going, you got this!");
        else if (msg == "?fact") await message.Channel.SendMessageAsync("🍌 Bananas are berries!");
        else if (msg == "?meme") await message.Channel.SendMessageAsync("😂 https://tenor.com/view/patrick-drooling-patrick-star-spongebob-movie-drooling-slime-gif-525340425450564005");
        else if (msg == "?time") await message.Channel.SendMessageAsync(DateTime.Now.ToShortTimeString());
        else if (msg == "?date") await message.Channel.SendMessageAsync(DateTime.Now.ToShortDateString());
        else if (msg == "?flip") await message.Channel.SendMessageAsync(rand.Next(2) == 0 ? "Heads" : "Tails");
        else if (msg == "?roll") await message.Channel.SendMessageAsync($"🎲 {rand.Next(1, 101)}");
        else if (msg == "?dice") await message.Channel.SendMessageAsync($"🎲 {rand.Next(1, 7)}");
        else if (msg == "?gamelink") await message.Channel.SendMessageAsync("THis is the game link for Steal a Custom BANANA!!! : https://www.roblox.com/share?code=7ef5c06850582548b308875d4edc8dd3&type=ExperienceDetails&stamp=1776427978985\n\n you can also go to the https://discord.com/channels/1478914840100868268/1494672140400726027 Channel to get it!");

        // =========================
        // OWNER CMDS
        // =========================
        else if (msg == "!rainty")
        {
            if (message.Author.Id != OWNER_ID)
            {
                await message.Channel.SendMessageAsync("❌ This CMD is for Rainyy Not you");
                return;
            }

            await message.Channel.SendMessageAsync("hi Rainyy");
        }
        else if (msg == "!cloudy")
        {
            if (message.Author.Id != other)
            {
                await message.Channel.SendMessageAsync("❌ This is cloudy not you");
                return;
            }

            await message.Channel.SendMessageAsync("hi Cloudy");
        }
        else if (msg == "!oak")
        {
            if (message.Author.Id != lul)
            {
                await message.Channel.SendMessageAsync("❌ This is Oakl.");
                return;
            }

            await message.Channel.SendMessageAsync("hi Oakly");
        }

        // =========================
        // FUN COMMANDS
        // =========================
        else if (msg == "?roast") await message.Channel.SendMessageAsync("You code like a potato 💀");
        else if (msg == "?compliment") await message.Channel.SendMessageAsync("You're actually amazing ⭐");
        else if (msg == "?8ball") await message.Channel.SendMessageAsync(Get8Ball());
        else if (msg == "?coinflip") await message.Channel.SendMessageAsync(rand.Next(2) == 0 ? "Heads" : "Tails");
        else if (msg == "?lucky") await message.Channel.SendMessageAsync($"🍀 {rand.Next(1, 100)}%");
        else if (msg == "?bruh") await message.Channel.SendMessageAsync("bruh 💀");

        // =========================
        // DM COMMAND (FIXED)
        // =========================
        else if (msg.StartsWith("?dm")) await HandleDM(message);

        // =========================
        // ADMIN ACTIONS (kept)
        // =========================
        else if (msg.StartsWith("?kick")) await Admin(message, "kick");
        else if (msg.StartsWith("?ban")) await Admin(message, "ban");
        else if (msg.StartsWith("?clear")) await Admin(message, "clear messages");
        else if (msg.StartsWith("?say")) await Admin(message, message.Content.Substring(5));
    }

    // =========================
    // FIXED DM SYSTEM
    // =========================
    private async Task HandleDM(SocketMessage message)
    {
        if (message.Author is not SocketGuildUser adminUser)
            return;

        if (!adminUser.Roles.Any(r => AdminRoles.Contains(r.Id)))
        {
            await message.Channel.SendMessageAsync("❌ No permission.");
            return;
        }

        var parts = message.Content.Split(' ', 3);

        if (parts.Length < 3 || message.MentionedUsers.Count == 0)
        {
            await message.Channel.SendMessageAsync("❌ Usage: ?dm @user message");
            return;
        }

        var target = message.MentionedUsers.First();
        string msgToSend = parts[2];

        if (msgToSend.StartsWith("'") && msgToSend.EndsWith("'"))
            msgToSend = msgToSend[1..^1];

        try
        {
            var dm = await target.CreateDMChannelAsync();
            await dm.SendMessageAsync(msgToSend);

            await message.Channel.SendMessageAsync($"✅ DM sent to {target.Username}");
        }
        catch
        {
            await message.Channel.SendMessageAsync("❌ User has DMs disabled.");
        }
    }

    // =========================
    // ADMIN CHECK (your system kept)
    // =========================
    private async Task Admin(SocketMessage message, string action)
    {
        if (message.Author is not SocketGuildUser user)
            return;

        if (!user.Roles.Any(r => AdminRoles.Contains(r.Id)))
        {
            await message.Channel.SendMessageAsync("❌ No permission (staff only).");
            return;
        }

        await message.Channel.SendMessageAsync(action);
    }

    // =========================
    // HELPERS
    // =========================
    private string GetJoke()
    {
        string[] jokes =
        {
            "Why is Rainyy so good at coding? IDK 😂",
            "I told my code a joke... it crashed 😂",
            "Stack overflow is my home 🏠"
        };

        return jokes[rand.Next(jokes.Length)];
    }

    private string Get8Ball()
    {
        string[] r =
        {
            "Yes",
            "No",
            "Maybe",
            "Definitely",
            "Ask again"
        };

        return r[rand.Next(r.Length)];
    }
}