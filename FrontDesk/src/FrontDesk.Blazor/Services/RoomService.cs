﻿using BlazorShared.Models.Room;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FrontDesk.Blazor.Services
{
    public class RoomService
    {
        private readonly HttpService _httpService;
        private readonly ILogger<RoomService> _logger;

        public RoomService(HttpService httpService, ILogger<RoomService> logger)
        {
            _httpService = httpService;
            _logger = logger;
        }

        public async Task<RoomDto> Create(RoomDto room)
        {
            return (await _httpService.HttpPost<CreateRoomResponse>("rooms", room)).Room;
        }

        public async Task<RoomDto> Edit(RoomDto room)
        {
            return (await _httpService.HttpPut<UpdateRoomResponse>("rooms", room)).Room;
        }

        public async Task<string> Delete(int roomId)
        {
            return (await _httpService.HttpDelete<DeleteRoomResponse>("rooms", roomId)).Status;
        }

        public async Task<RoomDto> GetById(int roomId)
        {
            return (await _httpService.HttpGet<GetByIdRoomResponse>($"rooms/{roomId}")).Room;
        }

        public async Task<List<RoomDto>> ListPaged(int pageSize)
        {
            _logger.LogInformation("Fetching rooms from API.");

            return (await _httpService.HttpGet<ListRoomResponse>($"rooms")).Rooms;
        }

        public async Task<List<RoomDto>> List()
        {
            _logger.LogInformation("Fetching rooms from API.");

            return (await _httpService.HttpGet<ListRoomResponse>($"rooms")).Rooms;
        }
    }
}
