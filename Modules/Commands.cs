using Discord;
using Discord.Interactions;
using InteractionFramework.Attributes;
using System;
using System.Threading.Tasks;

namespace InteractionFramework.Modules
{
    public class ExampleModule : InteractionModuleBase<SocketInteractionContext>
    {
        public InteractionService Commands { get; set; }

        private InteractionHandler _handler;
        public ExampleModule(InteractionHandler handler)
        {
            _handler = handler;
        }

        [SlashCommand("echo", "Repeat the input")]
        public async Task Echo(string echo, [Summary(description: "mention the user")]bool mention = false)
            => await RespondAsync(echo + (mention ? Context.User.Mention : string.Empty));

        [SlashCommand("ping", "Pings the bot and returns its latency.")]
        public async Task GreetUserAsync()
            => await RespondAsync(text: $":ping_pong: It took me {Context.Client.Latency}ms to respond to you!", ephemeral: true);

        [SlashCommand("bitrate", "Gets the bitrate of a specific voice channel.")]
        public async Task GetBitrateAsync([ChannelTypes(ChannelType.Voice, ChannelType.Stage)] IChannel channel)
            => await RespondAsync(text: $"This voice channel has a bitrate of {(channel as IVoiceChannel).Bitrate}");

        [Group("test_group", "This is a command group")]
        public class GroupExample : InteractionModuleBase<SocketInteractionContext>
        {
            [SlashCommand("choice_example", "Enums create choices")]
            public async Task ChoiceExample(ExampleEnum input)
                => await RespondAsync(input.ToString());
        }

        [ComponentInteraction("musicSelect:*,*")]
        public async Task ButtonPress(string id, string name)
        {
            // ...
            await RespondAsync($"Playing song: {name}/{id}");
        }

        [DoUserCheck]
        [ComponentInteraction("myButton:*")]
        public async Task ClickButtonAsync(string userId)
            => await RespondAsync(text: ":thumbsup: Clicked!");

        [UserCommand("greet")]
        public async Task GreetUserAsync(IUser user)
            => await RespondAsync(text: $":wave: {Context.User} said hi to you, <@{user.Id}>!");

        [MessageCommand("pin")]
        public async Task PinMessageAsync(IMessage message)
        {
            if (message is not IUserMessage userMessage)
                await RespondAsync(text: ":x: You cant pin system messages!");

            else if ((await Context.Channel.GetPinnedMessagesAsync()).Count >= 50)
                await RespondAsync(text: ":x: You cant pin any more messages, the max has already been reached in this channel!");

            else
            {
                await userMessage.PinAsync();
                await RespondAsync(":white_check_mark: Successfully pinned message!");
            }
        }

        [SlashCommand("embed", "Sends an embed")]
        public async Task Embed()
        {
            var embed = new EmbedBuilder()
                .WithTitle("Embed Title")
                .WithDescription("Embed Description")
                .WithColor(Color.Blue)
                .WithFooter("Embed Footer")
                .WithTimestamp(DateTimeOffset.Now)
                .Build();

            await RespondAsync(embed: embed);
        }
        [SlashCommand("warn", "Warns a user")]
        public async Task WarnUserAsync(IUser user, [Summary(description: "The reason for the warning")]string reason)
        {
            var embed = new EmbedBuilder()
                .WithTitle("Warning")
                .WithDescription($"You have been warned by {Context.User.Username}\n Reason: {reason}")
                .WithColor(Color.Red)
                .WithFooter("Warning Footer")
                .WithTimestamp(DateTimeOffset.Now)
                .Build();

            try
            {
                await user.SendMessageAsync(embed: embed);
                await RespondAsync(text: $":ok_hand: Successfully warned {user.Username}");
            }
            catch
            {
                await RespondAsync(text: ":x: Failed to send warning to user!");
            }
        }


    }
}
