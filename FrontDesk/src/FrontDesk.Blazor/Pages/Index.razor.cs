using BlazorShared.Models.Appointment;
using BlazorShared.Models.AppointmentType;
using BlazorShared.Models.Client;
using BlazorShared.Models.Patient;
using BlazorShared.Models.Room;
using FrontDesk.Blazor.Services;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telerik.Blazor;
using Telerik.Blazor.Components;
using Telerik.Blazor.Components.Scheduler;
using Telerik.Blazor.Components.Scheduler.Models;
using Telerik.Blazor.Components.Scheduler.ViewModels;

namespace FrontDesk.Blazor.Pages
{
    internal abstract class SchedulerDayViewModelBase : ISchedulerViewModel
    {
        public SchedulerDayViewModelBase(ISchedulerState state)
        {
        }

        private bool IsWorkHour(DateTime date)
        {
            return ((date >= this.WorkDayStart) && (date < this.WorkDayEnd));
        }

        public Dictionary<string, Dictionary<string, string>> GetResourceColors()
        {
            throw new NotImplementedException();
        }

        public abstract int NumberOfDays { get; }

        private SchedulerDayViewBase DayView
        {
            get
            {
                return (State.SelectedView as SchedulerDayViewBase);
            }
        }

        public int SlotDuration
        {
            get
            {
                return this.DayView.SlotDuration;
            }
        }

        public int SlotDivisions
        {
            get
            {
                return this.DayView.SlotDivisions;
            }
        }

        public DateTime NextPeriod
        {
            get
            {
                return State.Date.AddDays((double)this.NumberOfDays);
            }
        }

        public DateTime PrevPeriod
        {
            get
            {
                return State.Date.AddDays((double)(-1 * this.NumberOfDays));
            }
        }

        public DateTime RangeStart
        {
            get
            {
                return State.Date;
            }
        }

        public DateTime RangeEnd
        {
            get
            {
                return State.Date.AddDays((double)this.NumberOfDays);
            }
        }

        protected DateTime StartTime
        {
            get
            {
                return this.DayView.StartTime;
            }
        }

        protected DateTime EndTime
        {
            get
            {
                return this.DayView.EndTime;
            }
        }

        protected DateTime WorkDayStart
        {
            get
            {
                return this.DayView.WorkDayStart;
            }
        }

        protected DateTime WorkDayEnd
        {
            get
            {
                return this.DayView.WorkDayEnd;
            }
        }

        public bool ShowFooter
        {
            get
            {
                return true;
            }
        }

        public DateTime VisibleStartTime
        {
            get
            {
                return (State.ShowWorkHours ? this.WorkDayStart : this.StartTime);
            }
        }

        public DateTime VisibleEndTime
        {
            get
            {
                return (State.ShowWorkHours ? this.WorkDayEnd : this.EndTime);
            }
        }

        public List<DateTime> GetDates(DateTime RangeStart, DateTime RangeEnd, int num)
        {
            return new List<DateTime>();
        }

        public List<DateTime> Dates
        {
            get
            {
                return GetDates(this.RangeStart, this.RangeEnd, 1);
            }
        }

        public List<DaySlot> TimeSlots
        {
            get
            {
                List<DaySlot> list = new List<DaySlot>();
                DateTime visibleStartTime = this.VisibleStartTime;
                DateTime visibleEndTime = this.VisibleEndTime;
                int num = this.SlotDuration / this.SlotDivisions;
                while (visibleStartTime < visibleEndTime)
                {
                    DaySlot item = new DaySlot(visibleStartTime, visibleStartTime.AddMinutes((double)num), true, this.IsWorkHour(visibleStartTime));
                    item.IsHeader = true;
                    list.Add(item);
                    DateTime start = visibleStartTime;
                    int num2 = 1;
                    while (true)
                    {
                        if (num2 >= this.SlotDivisions)
                        {
                            visibleStartTime = visibleStartTime.AddMinutes((double)this.SlotDuration);
                            break;
                        }
                        start = start.AddMinutes((double)num);
                        if (start < visibleEndTime)
                        {
                            list.Add(new DaySlot(start, start.AddMinutes((double)num), num2 < (this.SlotDivisions - 1), this.IsWorkHour(start)));
                        }
                        num2++;
                    }
                }
                return list;
            }
        }

        public SchedulerView ViewType => throw new NotImplementedException();

        public Telerik.Blazor.Components.Scheduler.ISchedulerState State { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }

    internal class SchedulerDayViewModel : SchedulerDayViewModelBase
    {
        public SchedulerDayViewModel(ISchedulerState state) : base(state)
        {
        }

        public override int NumberOfDays
        {
            get
            {
                return 1;
            }
        }

        public SchedulerView ViewType
        {
            get
            {
                return SchedulerView.Day;
            }
        }
    }

    public class SchedulerState : ISchedulerState
    {
        DateTime ISchedulerState.Date => DateTime.Now;

        ISchedulerView ISchedulerState.SelectedView => null;

        bool ISchedulerState.ShowWorkHours => false;

        bool ISchedulerState.AllowCreate  => true;

        bool ISchedulerState.AllowUpdate  => true;

        bool ISchedulerState.AllowDelete => true;

        List<ISchedulerResource> ISchedulerState.ResourceCollection => null;
    }

    public interface ISchedulerState
    {
        DateTime Date { get; }

        ISchedulerView SelectedView { get; }

        bool ShowWorkHours { get; }

        bool AllowCreate { get; }

        bool AllowUpdate { get; }

        bool AllowDelete { get; }

        List<ISchedulerResource> ResourceCollection { get; }
    }

    public partial class Index
    {

        public static ISchedulerViewModel GetViewModel(ISchedulerState state)
        {
            if ((state != null) && (state.SelectedView != null))
            {
                switch (state.SelectedView.ViewType)
                {
                    case SchedulerView.Day:
                        return new SchedulerDayViewModel(state);

                    case SchedulerView.MultiDay:
                        return new SchedulerDayViewModel(state);

                    case SchedulerView.Week:
                        return new SchedulerDayViewModel(state);

                    case SchedulerView.Month:
                        return new SchedulerDayViewModel(state);

                    default:
                        break;
                }
            }
            return null;
        }

        ISchedulerViewModel modal = GetViewModel(new SchedulerState());

        List<Appointment> AppointmentItems = new List<Appointment>();

        [Inject]
        AppointmentService AppointmentService { get; set; }

        [Inject]
        AppointmentTypeService AppointmentTypeService { get; set; }

        [Inject]
        RoomService RoomService { get; set; }

        [Inject]
        PatientService PatientService { get; set; }

        [Inject]
        ClientService ClientService { get; set; }

        [Inject]
        FileService FileService { get; set; }

        [Inject]
        ConfigurationService ConfigurationService { get; set; }

        private List<AppointmentDto> Appointments = new List<AppointmentDto>();
        private List<AppointmentTypeDto> AppointmentTypes = new List<AppointmentTypeDto>();
        private List<ClientDto> Clients = new List<ClientDto>();
        private List<RoomDto> Rooms = new List<RoomDto>();
        private List<PatientDto> Patients = new List<PatientDto>();
        private List<PatientDto> ClientPatients => Patients.Where(p => p.ClientId == ClientId).ToList();

        private DateTime StartDate { get; set; } = new DateTime(2014, 6, 9, 7, 0, 0);
        private SchedulerView CurrView { get; set; } = SchedulerView.Week;

        private DateTime DayStart { get; set; } = new DateTime(2014, 6, 9, 7, 0, 0);
        private DateTime DayEnd { get; set; } = new DateTime(2014, 6, 9, 18, 00, 0);
        private DateTime WorkDayStart { get; set; } = new DateTime(2000, 1, 1, 7, 0, 0);
        private DateTime WorkDayEnd { get; set; } = new DateTime(2000, 1, 1, 18, 0, 0);

        private bool CustomEditFormShown { get; set; }
        AppointmentDto CurrentAppointment { get; set; } // we will put here a copy of the appointment for editing

        private DateTime Today { get; set; } = new DateTime();
        private int PatientId { get; set; } = 1;
        private int ClientId { get; set; } = 1;
        private int RoomId { get; set; } = 1;
        private PatientDto Patient { get; set; } = new PatientDto();
        private Guid ScheduleId
        {
            get
            {
                if (Appointments.Count > 0)
                {
                    return Appointments[0].ScheduleId;
                }

                return Guid.Empty;
            }
        }


        private bool CanMakeAppointment => IsRoomSelected && IsClientSelected && IsPatientSelected;
        private bool IsRoomSelected => RoomId > 0;
        private bool IsClientSelected => ClientId > 0;
        private bool IsPatientSelected => RoomId > 0 && ClientId > 0 && PatientId > 0;

        protected override async Task OnInitializedAsync()
        {
            Appointments = await AppointmentService.ListAsync();
            AppointmentTypes = await AppointmentTypeService.ListAsync();
            Clients = await ClientService.ListAsync();
            Rooms = await RoomService.ListAsync();

            Patients = await PatientService.ListAsync();
            Patient = Patients.FirstOrDefault(p => p.PatientId == PatientId);

            Today = await ConfigurationService.ReadAsync("TestDate");
            StartDate = UpdateDateToToday(StartDate);
            DayStart = UpdateDateToToday(DayStart);
            DayEnd = UpdateDateToToday(DayEnd);

            await AddPatientImages();
        }        

        private async Task CancelEditing()
        {
            await RefreshDataAsync();
        }

        private async Task RefreshDataAsync()
        {
            //an event callback needs to be raised in this component context to re-render the contents and to hide the dialog
            CustomEditFormShown = false;
            CurrentAppointment = null;
            //we also use it to fetch the fresh data from the service - in a real case other updates may have occurred
            //which is why I chose to use a separate event and not two-way binding. It is also used for refreshing on Cancel
            Appointments = await AppointmentService.ListAsync();
        }

        private void EditHandler(SchedulerEditEventArgs args)
        {
            args.IsCancelled = true;//prevent built-in edit form from showing up
            if (!CanMakeAppointment)
            {
                return;
            }

            AppointmentDto item = args.Item as AppointmentDto;
            CustomEditFormShown = true;
            if (!args.IsNew) // an edit operation, otherwise - an insert operation
            {
                //copying is implemented in the appointment model and it is needed because
                //this is a reference to the data collection so modifying it directly
                //will immediately modify the data and no cancelling will be possible
                CurrentAppointment = item.ShallowCopy();
            }
            else
            {
                CurrentAppointment = new AppointmentDto() { Start = args.Start, End = args.End, IsAllDay = args.IsAllDay };
            }
        }

        private async Task DeleteAppointmentAsync(SchedulerDeleteEventArgs args)
        {
            AppointmentDto item = (AppointmentDto)args.Item;
            await AppointmentService.DeleteAsync(item.AppointmentId);
            Appointments.Remove(item);
        }

        private async Task AddPatientImages()
        {
            foreach (var patient in Patients)
            {
                if (string.IsNullOrEmpty(patient.Name))
                {
                    continue;
                }

                var imgeData = await FileService.ReadPicture($"{patient.Name}.jpg");
                if (string.IsNullOrEmpty(imgeData))
                {
                    continue;
                }

                patient.ImageData = $"data:image/png;base64, {imgeData}";
            }
        }

        private void PatientChanged(int id)
        {
            PatientId = id;
            Patient = Patients.FirstOrDefault(p => p.PatientId == PatientId);
        }

        private DateTime UpdateDateToToday(DateTime date)
        {
            return new DateTime(Today.Year, Today.Month, Today.Day, date.Hour, date.Minute, date.Second);
        }

        private RenderFragment renderTextBox()
        {
            var xx = "";
            RenderFragment item = b => {
                b.OpenElement(0, "div");
                b.AddAttribute(1, "class", "k-widget k-scheduler k-scheduler-flex k-floatwrap telerik-blazor");
                b.AddMarkupContent(2, "\r\n    ");
                b.AddMarkupContent(3, b.ToString());
                b.CloseComponent();
            };
            Console.Write(xx);
            return item;
        }
    }
}
