using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using ATGV3.Business.History;
using ATGV3.Business.Model;
using ATGV3.Business.Repos;
using ATGV3.Business.Validators;
using ATGV3.Common;
using ATGV3.Common.EFUtil.Repository;
using Newtonsoft.Json;
using RefactorThis.GraphDiff;

namespace ATGV3.Business
{
    abstract public class BaseBusiness<T> : IBusiness<T> where T : class, new()
    {
        protected AtgRepo _repository = new AtgRepo();
        protected ATGV3Entities db;
        protected ApplicationServices _appServices;
        protected ApplicationServicesSetup _appServicesSetup;
        protected IValidation<T> _validate;
        protected IHistoryWriter<T> _historyWriter;
        protected AspNetUser _user;

        public BaseBusiness(ApplicationServicesSetup appServicesSetup, AspNetUser user)
        {
            _appServicesSetup = appServicesSetup;
            _appServices = new ApplicationServices(appServicesSetup);
            _repository.LazyLoading = appServicesSetup.LazyLoading;
            _repository.ProxyCreationEnabled = appServicesSetup.ProxyCreationEnabled;
            ValidationErrors = new List<string>();
            _user = user;
        }

        public AtgRepo GetRepository()
        {
            return _repository;
        }

        public List<String> ValidationErrors { get; set; }

        public virtual IEnumerable<T> Select()
        {
            return _repository.Select<T>();
        }

        public virtual IEnumerable<T> Select(Expression<Func<T, bool>> whereClause = null, string[] include = null, IOrderByClause<T>[] orderBy = null, bool entityTracking = true)
        {
            return this.Select(whereClause, orderBy, 0, 0, include, entityTracking);
        }

        public virtual IEnumerable<T> Select(Expression<Func<T, bool>> whereClause = null)
        {
            return this.Select(whereClause, null, null);
        }

        //depricated, phase out
        public virtual IEnumerable<T> Select(Expression<Func<T, bool>> whereClause = null, IOrderByClause<T>[] orderBy = null, int skip = 0, int top = 0, string[] include = null, bool entityTracking = true)
        {
            try
            {
                return _repository.Select(whereClause,include, orderBy, entityTracking,  skip, top);
            }
            catch (Exception ex)
            {
                if (_appServices.HandleException(ex, ExceptionCategory.Data)) throw;
            }
            return null;
        }

        public IEnumerable<T> SelectWithPermissions(Expression<Func<T, bool>> whereClause = null, string[] include = null, IOrderByClause<T>[] orderBy = null, bool entityTracking = true, int skip = 0, int top = 0)
        {
            try
            {
                IQueryable<T> query = _repository.Select(whereClause,include, orderBy, entityTracking,  skip, top).AsQueryable();

                if (PermissionsQuery != null)
                    return query.Where(PermissionsQuery);
                else
                    return query;
                
            }
            catch (Exception ex)
            {
                if (_appServices.HandleException(ex, ExceptionCategory.Data)) throw;
            }
            return null;
        }

        public IQueryable<T> QueryWithPermissions(string[] includes = null)
        {
            return _repository.SelectQueryable<T>(includes).Where(PermissionsQuery);
        }

        public IQueryable<T> Query(string[] includes = null)
        {
            return _repository.SelectQueryable<T>(includes);
        }

        protected Expression<Func<T, bool>> PermissionsQuery { get; set; }
        

        public abstract int Create(T item);
        public abstract bool Modify(T item, bool merge = false);

        public void Delete(T item, bool saveImmediately = true)
        {
            try
            {
                _repository.Delete(item, saveImmediately);
            }
            catch (Exception ex)
            {
                if (_appServices.HandleException(ex, ExceptionCategory.Data)) throw;
            }

        }

        public T InsertOpt(T item, bool saveImmediately = true)
        {
            try
            {
                item = _repository.Insert(item, saveImmediately);

                if (_historyWriter != null)
                {
                    _historyWriter.Write(item, _user.Id, Globals.HistoryActions.Create, null, "Created");
                }

            }
            catch (DbEntityValidationException ex)
            {
                var sb = new StringBuilder();
                foreach (var failure in ex.EntityValidationErrors)
                {
                    sb.AppendFormat("{0} failed validation", failure.Entry.Entity.GetType());
                    foreach (var error in failure.ValidationErrors)
                    {
                        sb.AppendFormat("- {0} : {1}", error.PropertyName, error.ErrorMessage);
                        sb.AppendLine();
                    }
                }

                throw new Exception(sb.ToString());
            }
            catch (Exception ex)
            {
                if (_appServices.HandleException(ex, ExceptionCategory.Data)) throw;
            }
            return item;
        }

        public T Insert(T item)
        {
            try
            {
                item = InsertOpt(item, true);
            }
            catch (Exception ex)
            {
                if (_appServices.HandleException(ex, ExceptionCategory.Data)) throw;
            }
            return item;
        }

        public void Save()
        {
            try
            {
                _repository.Save();
            }
            catch (DbEntityValidationException ex)
            {
                var sb = new StringBuilder();
                foreach (var failure in ex.EntityValidationErrors)
                {
                    sb.AppendFormat("{0} failed validation", failure.Entry.Entity.GetType());
                    foreach (var error in failure.ValidationErrors)
                    {
                        sb.AppendFormat("- {0} : {1}", error.PropertyName, error.ErrorMessage);
                        sb.AppendLine();
                    }
                }

                _appServices.LogWriteFull(sb.ToString(), LoggingSeverity.Important, LoggingCategory.Data);
                throw new Exception(sb.ToString());
            }
            catch (Exception ex)
            {
                if (_appServices.HandleException(ex, ExceptionCategory.Data)) throw;
            }
        }

        public void UpdateOpt(T item, bool saveImmediately = true)
        {
            try
            {
                if (_historyWriter != null)
                {
                    _historyWriter.WriteUpdateChanges(item, _repository.GetDBContext(), _user.Id);
                }
                _repository.Update(item, saveImmediately);
            }
            catch (DbEntityValidationException ex)
            {
                var sb = new StringBuilder();
                foreach (var failure in ex.EntityValidationErrors)
                {
                    sb.AppendFormat("{0} failed validation", failure.Entry.Entity.GetType());
                    foreach (var error in failure.ValidationErrors)
                    {
                        sb.AppendFormat("- {0} : {1}", error.PropertyName, error.ErrorMessage);
                        sb.AppendLine();
                    }
                }

                _appServices.LogWriteFull(sb.ToString(), LoggingSeverity.Important, LoggingCategory.Data);
                throw new Exception(sb.ToString());

            }
            catch (Exception ex)
            {
                if (_appServices.HandleException(ex, ExceptionCategory.Data)) throw;
            }
        }

        //update the the item with changes passed in
        public void Update(T item)
        {
            try
            {
                UpdateOpt(item, true);
            }

            catch (Exception ex)
            {
                if (_appServices.HandleException(ex, ExceptionCategory.Data)) throw;
            }
        } 


        //merge passed in changes with changes from the database
        public T UpdateMerge(T item, Expression<Func<IUpdateConfiguration<T>, object>> expression)
        {
            try
            {
               var newItem = _repository.UpdateGraph(item,expression,false);

               if (_historyWriter != null)
                   _historyWriter.WriteUpdateChanges(newItem,  _repository.GetDBContext(), _user.Id);
               
               _repository.Save();
               return newItem;
            }
            catch (Exception ex)
            {
                _appServices.HandleException(ex, ExceptionCategory.Data);

                //history writing failed, save object
                return _repository.UpdateGraph(item, expression, true);
            }

            return item;
        }

        public void BulkUpdate(IEnumerable<T> items)
        {
            try
            {
                foreach (var item in items)
                {
                    _repository.Update(item, false);
                }

                _repository.Save();
            }
            catch (Exception ex)
            {
                if (_appServices.HandleException(ex, ExceptionCategory.Data)) throw;
            }
        }

        public bool Validate(T item)
        {
            if (_validate != null)
            {
                List<string> returnErrors = _validate.Validate(item);
                if (returnErrors.Count > 0)
                {
                    ValidationErrors.AddRange(returnErrors);
                    return false;
                }
            }

            return true;
        }


        public void Dispose()
        {
            _repository = null;
            _appServices = null;
        }

        internal IEnumerable<int> GetUserGroupIds()
        {
            return  _user.Groups.Select(g => g.Id);
        }

        //can a user access this particular customer
        protected bool CustomerPermissionCheck(int CustomerId)
        {
            Customer c = this.GetRepository().Select<Customer>(i => i.Id == CustomerId, new string[] { Globals.Related.Customer.Groups }).FirstOrDefault();

            var groups = GetUserGroupIds();
            return (groups.Any(groupId => c.Groups.Any(g => g.Id == groupId)) || c.AccountManager == _user.Id || c.SalesManager == _user.Id);
        }

        public bool NotifyUser(string message, int UserId, string context = null)
        {
            return AddNotification(message, new List<int> { UserId }, context);
        }

        //notify system users
        public bool NotifyUsers(string message, IEnumerable<int> UserIds, string context = null)
        {
            return AddNotification(message, UserIds, context);
        }

        //notify members of a specific group
        public bool NotifyGroup(string message, int GroupId, string context = null)
        {
            Group g = _repository.Select<Group>(i => i.Id == GroupId, new[] { "AspNetUsers" }).FirstOrDefault();
            return AddNotification(message, g.Members.Select(i => i.Id), context);
        }

        //add a system notification for an entire office
        public bool NotifyOffice(string message, int OfficeId, string context = null)
        {
            Group g = _repository.Select<Group>(i => i.OfficeId == OfficeId && i.Name == Globals.OFFICE_GROUP_NAME, new[] { "AspNetUsers" }).FirstOrDefault();

            return AddNotification(message, g.Members.Select(i => i.Id), context);
        }

        //add a system notification that will alert the user
        private bool AddNotification(string message, IEnumerable<int> RecipientUserIds, string context = null)
        {
            if (!RecipientUserIds.Any())
                return false;

            var Notification = this._repository.Insert<AppMessage>(new AppMessage()
            {
                Id = 0,
                CreatedBy = _user.Id,
                CreatedDate = DateTime.Now,
                Message = message,
                MessageType = Globals.AppMessageTypes.Notification,
                Recipient = RecipientUserIds.First()
            });

        
            foreach (int userId in RecipientUserIds)
            {
                this._repository.Insert<AppMessageUser>(new AppMessageUser()
                {
                    UserId = userId,
                    MessageId = Notification.Id,
                }, false);
            }

            _repository.Save();
            
            Notification.CreatedByUser = _user;
            ApplicationEvent.Instance.DispatchUserEvent(Globals.ApplicationEvent.NewNotification, RecipientUserIds.ToList(), Notification.Serialize(_user), context);
            return true;
        }

        public Reminder SetReminder(Reminder reminder, bool saveImmediately = true)
        {
            if (reminder.SetFor == 0)
                reminder.SetFor = _user.Id;

            reminder.SetBy = _user.Id;

            if (_user.IsAgent())
                reminder.Public = false;

            //if new reminder
            if (reminder.Id == 0)
            {
                reminder.CreatedDate = DateTime.Now;
                _repository.Insert<Reminder>(reminder, saveImmediately);

                _DispatchReminderEvent(reminder.Id, ReminderEvent.Create);
            }

            //else update reminder
            else
            {
                reminder.SetByUser = null;
                reminder.SetForUser = null;

                _repository.Update<Reminder>(reminder, saveImmediately);
                _DispatchReminderEvent(reminder.Id, ReminderEvent.Update);
            }

            return this.GetReminder(reminder.Id);
        }

        private enum ReminderEvent
        {
            Create,
            Update,
            Delete
        }

        public void DeleteReminder(Reminder reminder)
        {
            reminder.SetByUser = null;
            reminder.SetForUser = null;
            _repository.Delete<Reminder>(reminder);
            _DispatchReminderEvent(reminder.Id, ReminderEvent.Delete);
            
            //signalR event etc. for deletion of a reminder
        }

        private void _DispatchReminderEvent(int reminderId, ReminderEvent action)
        {
            string Event = null;
            switch (action)
            {
                case ReminderEvent.Create:
                    Event = Globals.ApplicationEvent.NewReminder;
                    break;

                case ReminderEvent.Update:
                    Event = Globals.ApplicationEvent.ReminderUpdate;
                    break;

                case ReminderEvent.Delete:
                    Event = Globals.ApplicationEvent.ReminderDeleted;
                    break;
            }

            if (action == ReminderEvent.Delete)
            {
                ApplicationEvent.Instance.DispatchOfficeEvent(Event, _user.OfficeId, new {ReminderId = reminderId});
            }
            else
            {
                Reminder reminder = GetReminder(reminderId);

                if (reminder.Public)
                    ApplicationEvent.Instance.DispatchOfficeEvent(Event, _user.OfficeId, reminder.Serialize());
                else
                    ApplicationEvent.Instance.DispatchUserEvent(Event, new List<int>() { reminder.SetFor }, reminder);
            }
      }

        public Reminder GetReminder(int id)
        {
            return _repository.Select<Reminder>(i => i.Id == id, new[] { "SetForUser", "SetByUser" }).FirstOrDefault();
        }

    }
}