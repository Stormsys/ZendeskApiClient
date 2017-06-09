﻿using System.Collections.Generic;
using System.Threading.Tasks;
using ZendeskApi.Client.Models;
using ZendeskApi.Client.Responses;

namespace ZendeskApi.Client.Resources
{
    public interface ITicketFieldsResource
    {
        Task<IEnumerable<TicketField>> GetAllAsync();
        Task<TicketField> GetAsync(long ticketFieldId);
        Task<TicketField> PostAsync(TicketField ticketField);
        Task<TicketField> PutAsync(TicketField ticketField);
        Task DeleteAsync(long ticketFieldId);
    }
}