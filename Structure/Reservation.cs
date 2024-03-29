﻿using System;
using System.Threading;
using System.Threading.Tasks;

namespace CafeSystem.Structure
{
    public class Reservation
    {
        /// <summary>
        /// Частота обновления
        /// </summary>
        public int Frequency { get; set; } = 1000; // = Каждую секунду будет обновлятся главный поток.

        public delegate void ReservationHandler(Reservation reservation, Computer pc);
        internal Computer ReservedPC;
        internal User User;

        public Reservation(TimeSpan duration, Computer pc)
        {
            Duration = duration;
            StartTime = DateTime.Now;
            EndTime = StartTime + Duration;
            Remaining = Duration;
            ReservedPC = pc;

            CheckLoop();
        }
        
        public Reservation(DateTime from, TimeSpan duration, Computer pc)
        {
            Duration = duration;
            StartTime = from;
            EndTime = StartTime + Duration;
            Remaining = Duration;
            ReservedPC = pc;
            

            CheckLoop();
        }

        /// <summary>
        /// Срабатывает, когда бронь начинает действовать
        /// </summary>
        public event ReservationHandler On_ReservationStarted;

        /// <summary>
        /// Срабатывает, когда бронь заканчивает своё действие
        /// </summary>
        public event ReservationHandler On_ReservationEnded;

        private async Task CheckLoop()
        {
            
            await Task.Run(() =>
            {
                if (ReservedPC.Reserved)
                {
                    LogBox.Log($"");
                }
                //Status = Status.Waiting;
                while (StartTime > DateTime.Now) Thread.Sleep(1000);

                On_ReservationStarted?.Invoke(this, ReservedPC);
                LogBox.Log($"Компьютер \"{ReservedPC}\" забронирован пользователем \"{ReservedPC.User}\". Детали бронирования: {this}");

                //Status = Status.Active;
                while (DateTime.Now < EndTime) Thread.Sleep(1000);
                ReservedPC.User.VisitedTime += (DateTime.Now - StartTime).TotalSeconds;
                ReservedPC.Reserved = false;
                

                On_ReservationEnded?.Invoke(this, ReservedPC);
                ReservedPC.User = null;
            });
        }

        //public async void Start()
        //{
        //    await CheckLoop();
        //}
        
        #region Всё что касается непосредственно брони

        /// <summary>
        ///     Время, в которое бронь начала действовать
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        ///     Длительность брони
        /// </summary>
        public TimeSpan Duration { get; set; }

        /// <summary>
        ///     Время, в которое бронь заканчивается
        /// </summary>
        public DateTime EndTime { get; set; }

        /// <summary>
        ///     Оставшееся время брони
        /// </summary>
        public TimeSpan Remaining { get; set; }


        #endregion

        public override string ToString()
        {
            return $"Время бронирования: [{StartTime.ToString("HH:mm:ss")} - {EndTime.ToString("HH:mm:ss")}]\n" +
                   $"Длительность брони: {Duration}";
        }
    }
}