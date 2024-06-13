//using Coaction.KickAssCardBot.Enums;
//using Coaction.KickAssCardBot.Models;

//namespace Coaction.KickAssCardBot.Manager
//{
//    public class MoversShakersManager : RavenConnectionManager
//    {
//        private const string RegisteredServersFileName = "AllRegisteredServers";

//        public async Task<DiscordServerChannelModel> AddChannelFormat(DiscordServerChannelModel serverInformation, MtgFormatsEnum formatName)
//        {
//            var listFormats = serverInformation.ListOfFormats ?? new List<string>();
//            var convertedFormat = formatName.ToString();

//            if (!listFormats.Contains(convertedFormat))
//            {
//                listFormats.Add(convertedFormat);
//            }

//            serverInformation.ListOfFormats = listFormats;
//            return await UpdateServerInfo(serverInformation);
//        }

//        public async Task<DiscordServerChannelModel> RemoveChannelFormat(DiscordServerChannelModel serverInformation, MtgFormatsEnum formatName)
//        {
//            var listFormats = serverInformation.ListOfFormats ?? new List<string>();
//            var convertedFormat = formatName.ToString();

//            if (listFormats.Contains(convertedFormat))
//            {
//                listFormats.Remove(convertedFormat);
//            }

//            serverInformation.ListOfFormats = listFormats;
//            return await UpdateServerInfo(serverInformation);
//        }

//        public async Task<DiscordServerChannelModel> ReadMoversShakersConfig(ulong serverId)
//        {
//            try
//            {
//                using var session = Store.OpenAsyncSession();
//                var discordConfig = await session.LoadAsync<DiscordServerChannelModel>($"{serverId}");
//                if (discordConfig != null)
//                {
//                    return discordConfig;
//                }

//                return new DiscordServerChannelModel
//                {
//                    ListOfFormats = new List<string>(),
//                    ServerId = serverId,
//                    ChannelId = 0,
//                };
//            }
//            catch (Exception e)
//            {
//                Logger.Log.Debug(e, $"Unable to get discord server {serverId}'s movers and shakers config due to {e.Message}");
//                throw;
//            }
//        }

//        public async Task<DiscordServerChannelModel> UpdateServerInfo(DiscordServerChannelModel serverInformation)
//        {
//            using var session = Store.OpenAsyncSession();
//            await session.StoreAsync(serverInformation, $"{serverInformation.ServerId}");
//            await session.SaveChangesAsync();

//            await UpdateListOfRegisteredGuilds(serverInformation);
//            return serverInformation;
//        }

//        private async Task UpdateListOfRegisteredGuilds(DiscordServerChannelModel serverInformation)
//        {
//            var currentServerInfo = await GetRegisteredServers();

//            currentServerInfo.ListOfRegisteredDiscordGuilds ??= new List<ulong>();

//            if (!currentServerInfo.ListOfRegisteredDiscordGuilds.Contains(serverInformation.ServerId))
//            {
//                currentServerInfo.ListOfRegisteredDiscordGuilds.Add(serverInformation.ServerId);
//            }

//            using var session = Store.OpenAsyncSession();
//            await session.StoreAsync(currentServerInfo, RegisteredServersFileName);
//            await session.SaveChangesAsync();
//        }

//        public async Task<MoversAndShakersServerInfoDataModel> GetRegisteredServers()
//        {
//            try
//            {
//                using var session = Store.OpenAsyncSession();
//                var registeredServers = await session.LoadAsync<MoversAndShakersServerInfoDataModel>(RegisteredServersFileName);

//                if (registeredServers != null)
//                {
//                    return registeredServers;
//                }

//                return new MoversAndShakersServerInfoDataModel
//                {
//                    ListOfRegisteredDiscordGuilds = new List<ulong>()
//                }; 
//            }
//            catch (Exception e)
//            {
//                Logger.Log.Debug(e, $"Failed to get registered servers due to {e.Message}");
//                throw;
//            }
//        }

//        public MoversShakersManager(string friendlyCertificateName, string dataBaseName) : base(friendlyCertificateName, dataBaseName)
//        {
//        }
//    }
//}
