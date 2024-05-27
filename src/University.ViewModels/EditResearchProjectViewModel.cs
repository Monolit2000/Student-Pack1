﻿using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using University.Data;
using University.Interfaces;
using University.Models;

namespace University.ViewModels
{
    public class EditResearchProjectViewModel : ViewModelBase, IDataErrorInfo
    {
        private readonly UniversityContext _context;
        private readonly IDialogService _dialogService;
        private ResearchProject? _researchProject = new ResearchProject();

        public string Error => string.Empty;



        #region base prop

        public string this[string columnName]
        {
            get
            {
                if (columnName == "Title")
                {
                    if (string.IsNullOrEmpty(Title))
                    {
                        return "Title is Required";
                    }
                }
                if (columnName == "Description")
                {
                    if (string.IsNullOrEmpty(Description))
                    {
                        return "Description is Required";
                    }
                }
                if (columnName == "StartDate")
                {
                    if (StartDate is null)
                    {
                        return "Start Date is Required";
                    }
                }
                if (columnName == "EndDate")
                {
                    if (EndDate is null)
                    {
                        return "End Date is Required";
                    }
                }
                if (columnName == "Budget" && Budget <= 0)
                {
                    return "Budget should be greater than 0";
                }
                return string.Empty;
            }
        }

        private string _title = string.Empty;
        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                OnPropertyChanged(nameof(Title));
            }
        }

        private string _description = string.Empty;
        public string Description
        {
            get => _description;
            set
            {
                _description = value;
                OnPropertyChanged(nameof(Description));
            }
        }

        private DateTime? _startDate = null;
        public DateTime? StartDate
        {
            get => _startDate;
            set
            {
                _startDate = value;
                OnPropertyChanged(nameof(StartDate));
            }
        }

        private DateTime? _endDate = null;
        public DateTime? EndDate
        {
            get => _endDate;
            set
            {
                _endDate = value;
                OnPropertyChanged(nameof(EndDate));
            }
        }

        private float _budget;
        public float Budget
        {
            get => _budget;
            set
            {
                _budget = value;
                OnPropertyChanged(nameof(Budget));
            }
        }

        private string _response = string.Empty;
        public string Response
        {
            get
            {
                return _response;
            }
            set
            {
                _response = value;
                OnPropertyChanged(nameof(Response));
            }
        }

        private long _researchProjectId = 0;
        public long ResearchProjectId
        {
            get => _researchProjectId;
            set
            {
                _researchProjectId = value;
                OnPropertyChanged(nameof(ResearchProjectId));
                LoadResearchProjectData();
            }
        }

        #endregion


        #region Navigations


        private ICommand? _back = null;
        public ICommand Back
        {
            get
            {
                if (_back is null)
                {
                    _back = new RelayCommand<object>(NavigateBack);
                }
                return _back;
            }
        }

        private void NavigateBack(object? obj)
        {
            var instance = MainWindowViewModel.Instance();
            if (instance is not null)
            {
                instance.ResearchProjectSubView = new ResearchProjectViewModel(_context, _dialogService);
            }
        }


      

        private ICommand? _save = null;
        public ICommand Save
        {
            get
            {
                if (_save is null)
                {
                    _save = new RelayCommand<object>(SaveData);
                }
                return _save;
            }
        }
        private void SaveData(object? obj)
        {
            if (!IsValid())
            {
                Response = "Please complete all required fields";
                return;
            }

            if (_researchProject is null)
            {
                return;
            }

            _researchProject.Title = Title;
            _researchProject.Description = Description;
            _researchProject.StartDate = StartDate.Value;
            _researchProject.EndDate = EndDate.Value;
            _researchProject.Budget = Budget;

            _context.Entry(_researchProject).State = EntityState.Modified;
            _context.SaveChanges();

            Response = "Data Saved";
        }


        #endregion



        public EditResearchProjectViewModel(UniversityContext context, IDialogService dialogService)
        {
            _context = context;
            _dialogService = dialogService;
        }

        private ObservableCollection<Student> LoadStudents()
        {
            _context.Database.EnsureCreated();
            _context.Students.Load();
            return _context.Students.Local.ToObservableCollection();
        }

        private bool IsValid()
        {
            string[] properties = { "Title", "Description", "StartDate", "EndDate", "Budget" };
            foreach (string property in properties)
            {
                if (!string.IsNullOrEmpty(this[property]))
                {
                    return false;
                }
            }
            return true;
        }

        private void LoadResearchProjectData()
        {
            var researchProjects = _context.ResearchProjects;
            if (researchProjects is not null)
            {
                _researchProject = researchProjects.Find(ResearchProjectId);
                if (_researchProject is null)
                {
                    return;
                }
                this.Title = _researchProject.Title;
                this.Description = _researchProject.Description;
                this.StartDate = _researchProject.StartDate;
                this.EndDate = _researchProject.EndDate;
                this.Budget = _researchProject.Budget;
            }
        }
    }
}
