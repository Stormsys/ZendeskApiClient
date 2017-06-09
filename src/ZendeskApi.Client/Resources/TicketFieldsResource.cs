﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ZendeskApi.Client.Models;
using ZendeskApi.Client.Requests;
using ZendeskApi.Client.Responses;

namespace ZendeskApi.Client.Resources
{
    public class TicketFieldsResource : ITicketFieldsResource
    {
        private const string ResourceUri = "api/v2/ticket_fields";

        private readonly IZendeskApiClient _apiClient;
        private readonly ILogger _logger;

        private Func<ILogger, string, IDisposable> _loggerScope =
            LoggerMessage.DefineScope<string>("TicketFieldResource: {0}");

        public TicketFieldsResource(IZendeskApiClient apiClient,
            ILogger logger)
        {
            _apiClient = apiClient;
            _logger = logger;
        }

        public async Task<IEnumerable<TicketField>> GetAllAsync()
        {
            using (_loggerScope(_logger, "GetAllAsync"))
            using (var client = _apiClient.CreateClient())
            {
                var response = await client.GetAsync(ResourceUri).ConfigureAwait(false);

                response.EnsureSuccessStatusCode();

                return (await response.Content.ReadAsAsync<TicketFieldsResponse>()).Item;
            }
        }

        public async Task<TicketField> GetAsync(long ticketFieldId)
        {
            using (_loggerScope(_logger, $"GetAsync({ticketFieldId})"))
            using (var client = _apiClient.CreateClient(ResourceUri))
            {
                var response = await client.GetAsync(ticketFieldId.ToString()).ConfigureAwait(false);

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    _logger.LogInformation("Ticket Field {0} not found", ticketFieldId);
                    return null;
                }

                response.EnsureSuccessStatusCode();

                return (await response.Content.ReadAsAsync<TicketFieldResponse>()).Item;
            }
        }

        public async Task<TicketField> PostAsync(TicketField ticketField)
        {
            using (_loggerScope(_logger, $"PostAsync"))
            using (var client = _apiClient.CreateClient())
            {
                var response = await client.PostAsJsonAsync(ResourceUri, new TicketFieldRequest { Item = ticketField }).ConfigureAwait(false);

                if (response.StatusCode != System.Net.HttpStatusCode.Created)
                {
                    throw new HttpRequestException(
                        $"Status code retrieved was {response.StatusCode} and not a 201 as expected" +
                        Environment.NewLine +
                        "See: https://developer.zendesk.com/rest_api/docs/core/ticket_fields#create-ticket-field");
                }

                return (await response.Content.ReadAsAsync<TicketFieldResponse>()).Item;
            }
        }

        public async Task<TicketField> PutAsync(TicketField ticketField)
        {
            using (_loggerScope(_logger, $"PutAsync"))
            using (var client = _apiClient.CreateClient(ResourceUri))
            {
                var response = await client.PutAsJsonAsync(ticketField.Id.ToString(), new TicketFieldRequest { Item = ticketField }).ConfigureAwait(false);

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    _logger.LogInformation("Cannot update ticket field as ticket field {0} cannot be found", ticketField.Id);
                    return null;
                }

                response.EnsureSuccessStatusCode();

                return (await response.Content.ReadAsAsync<TicketFieldResponse>()).Item;
            }
        }

        public async Task DeleteAsync(long ticketFieldId)
        {
            using (_loggerScope(_logger, $"DeleteAsync({ticketFieldId})"))
            using (var client = _apiClient.CreateClient(ResourceUri))
            {
                var response = await client.DeleteAsync(ticketFieldId.ToString());

                if (response.StatusCode != System.Net.HttpStatusCode.NoContent)
                {
                    throw new HttpRequestException(
                        $"Status code retrieved was {response.StatusCode} and not a 204 as expected" +
                        Environment.NewLine +
                        "See: https://developer.zendesk.com/rest_api/docs/core/tickets#delete-ticket");
                }
            }
        }
    }
}