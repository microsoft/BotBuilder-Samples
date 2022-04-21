// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license.
using System.Threading;
using System.Threading.Tasks;

namespace Bot.Builder.Azure.V3V4
{
    /// <summary>
    /// From https://github.com/microsoft/BotBuilder-Azure
    /// </summary>
    /// <typeparam name="T"><see cref="BotData"/></typeparam>
    public interface IBotDataStore<T>
    {
        Task<bool> FlushAsync(IAddress key, CancellationToken cancellationToken);
        //
        // Summary:
        //     Return BotData with Data pointing to a JObject or an empty BotData() record with
        //     ETag:""
        //
        // Parameters:
        //   key:
        //     The key.
        //
        //   botStoreType:
        //     The bot store type.
        //
        //   cancellationToken:
        //     The cancellation token.
        //
        // Returns:
        //     Bot record that is stored for this key, or "empty" bot record ready to be stored
        Task<T> LoadAsync(IAddress key, BotStoreType botStoreType, CancellationToken cancellationToken);
        //
        // Summary:
        //     Save a BotData using the ETag. Etag consistency checks If ETag is null or empty,
        //     this will set the value if nobody has set it yet If ETag is "*" then this will
        //     unconditionally set the value If ETag matches then this will update the value
        //     if it is unchanged. If Data is null this removes record, otherwise it stores
        //
        // Parameters:
        //   key:
        //     The key.
        //
        //   botStoreType:
        //     The bot store type.
        //
        //   data:
        //     The data that should be saved.
        //
        //   cancellationToken:
        //     The cancellation token.
        //
        // Returns:
        //     throw HttpException(HttpStatusCode.PreconditionFailed) if update fails
        Task SaveAsync(IAddress key, BotStoreType botStoreType, T data, CancellationToken cancellationToken);
    }
}
