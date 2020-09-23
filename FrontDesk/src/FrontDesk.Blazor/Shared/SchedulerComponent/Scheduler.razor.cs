using BlazorShared.Models.Appointment;
using BlazorShared.Models.AppointmentType;
using BlazorShared.Models.Room;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telerik.Blazor.Components;

namespace FrontDesk.Blazor.Shared.SchedulerComponent
{

    public partial class Scheduler
    {
        [Inject]
        IJSRuntime JSRuntime { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }
        [Parameter]
        public RenderFragment<Resource> RenderFragmentResources { get; set; }

        [Parameter] public List<string> Groups { get; set; } = new List<string>();
        [Parameter] public DateTime StartDate { get; set; } = new DateTime();
        [Parameter] public DateTime StartTime { get; set; } = new DateTime();
        [Parameter] public DateTime EndTime { get; set; } = new DateTime();
        [Parameter] public List<RoomDto> Rooms { get; set; } = new List<RoomDto>();
        [Parameter] public List<AppointmentDto> Appointments { get; set; } = new List<AppointmentDto>();
        [Parameter] public List<AppointmentTypeDto> AppointmentTypes { get; set; } = new List<AppointmentTypeDto>();
        [Parameter] public List<SchedulerResourceModel> SchedulerResources { get; set; } = new List<SchedulerResourceModel>();

        [Parameter]
        public EventCallback<string> OnEditCallback { get; set; }

        private List<SchedulerResourceModel> Resources { get; set; } = new List<SchedulerResourceModel>();

        private string EditButtonId => "editFire";

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await CallJSMethod();
                await JSRuntime.InvokeVoidAsync("addListenerToFireEdit", EditButtonId);
            }
            await base.OnAfterRenderAsync(firstRender);
        }

        public void AddResource(Resource resourceComponent)
        {
            Resources.Add(resourceComponent.SchedulerResource);
            StateHasChanged();
        }

        private async Task CallJSMethod()
        {
            await JSRuntime.InvokeVoidAsync("scheduler", StartDate, StartTime, EndTime, Appointments, Rooms, AppointmentTypes, Resources, Groups);            
        }

        private async Task OpenEdit()
        {
            await OnEditCallback.InvokeAsync();
        }
    }
}
