﻿using System.Text;
using System.Windows.Input;
using Assignment.Application.Common.Exceptions;
using Assignment.Application.Common.Interfaces;
using Assignment.Application.TodoItems.Commands.DoneTodoItem;
using Assignment.Application.TodoLists.Queries.GetTodos;
using Assignment.UI.Cache;
using Caliburn.Micro;
using MediatR;

namespace Assignment.UI
{
    //TODO: Find way to remove async void for comand 
    internal class TodoManagmentViewModel : Screen
    {
        private readonly ISender _sender;
        private readonly IWindowManager _windowManager;
        private readonly ISimpleCache<TodoListDto> _todoListCache;

        //ObservableCollection is better than IList
        private NotifyTaskCompletion<IList<TodoListDto>> _todoLists;
        public NotifyTaskCompletion<IList<TodoListDto>> TodoLists
        {
            get => _todoLists;
            set
            {
                _todoLists = value;
                NotifyOfPropertyChange(() => TodoLists);
            }
        }

        private TodoListDto _selectedTodoList;
        public TodoListDto SelectedTodoList
        {
            get => _selectedTodoList;
            set
            {
                _selectedTodoList = value;
                NotifyOfPropertyChange(() => SelectedTodoList);
            }
        }

        public IEnumerable<TodoItemDto> SelectedTodoListItems => SelectedTodoList?.Items;

        private TodoItemDto _selectedItem;
        public TodoItemDto SelectedItem
        {
            get => _selectedItem;
            set
            {
                _selectedItem = value;
            }
        }

        public ICommand AddTodoListCommand { get; private set; }
        public ICommand AddTodoItemCommand { get; private set; }
        public ICommand DoneTodoItemCommand { get; private set; }

        public TodoManagmentViewModel(ISender sender, IWindowManager windowManager)
        {
            _sender = sender;
            _windowManager = windowManager;
            _todoListCache = new SimpleCache<TodoListDto>(TimeSpan.FromMinutes(10));

            AddTodoListCommand = new RelayCommand(AddTodoList);
            AddTodoItemCommand = new RelayCommand(AddTodoItem, CanExecuteAddTodoItem);
            DoneTodoItemCommand = new RelayCommand(DoneTodoItem, CanExecuteDoneTodoItem);
            RefreshTodoLists();
            foreach (var todoList in TodoLists.Result)
            {
                _todoListCache.Set(todoList.Id.ToString(), todoList);
            }
        }

        private void RefreshTodoLists()
        {
            var selectedListId = SelectedTodoList?.Id;
            TodoLists = new NotifyTaskCompletion<IList<TodoListDto>>(_sender.Send(new GetTodosQuery()));
            if (selectedListId.HasValue && selectedListId.Value > 0)
            {
                var selectedList = TodoLists.Result.FirstOrDefault(list => list.Id == selectedListId.Value);
                SelectedTodoList = selectedList;
            }
        }

        private async void AddTodoList(object obj)
        {
            var todoList = new TodoListViewModel(_sender);
            try
            {
                var result = await _windowManager.ShowDialogAsync(todoList);
                if (result == true)
                {
                    //var newList = todoList.GetTodoList();
                    //_todoListCache.Set(newList.Id.ToString(), newList);

                    RefreshTodoLists();
                }
            }
            catch (ValidationException ex)
            {
                ShowError(FormatErrors(ex));
            }
        }

        private async void AddTodoItem(object obj)
        {
            var todoItem = new TodoItemViewModel(_sender, SelectedTodoList.Id);
            try
            {
                var result = await _windowManager.ShowDialogAsync(todoItem);
                if (result is true)
                {
                    //var updatedList = await _todoListCache.Get(SelectedTodoList.Id.ToString());
                    //if (updatedList != null)
                    //{
                    //    updatedList.Items.Add(todoItem.GetTodoItem());
                    //    _todoListCache.Set(updatedList.Id.ToString(), updatedList);
                    //}

                    RefreshTodoLists();
                }
            }
            catch (ValidationException ex)
            {
                ShowError(FormatErrors(ex));
            }
        }

        private async void DoneTodoItem(object obj)
        {
            if (SelectedItem is null)
            {
                return;
            }

            await _sender.Send(new DoneTodoItemCommand(SelectedItem.Id));
            RefreshTodoLists();
        }

        private bool CanExecuteDoneTodoItem(object obj)
        {
            return SelectedItem != null && !SelectedItem.Done;
        }

        private bool CanExecuteAddTodoItem(object obj)
        {
            return SelectedTodoList != null;
        }

        public void ShowError(string errorMessage)
        {
            var viewModel = new ValidationErrorViewModel
            {
                ErrorMessage = errorMessage
            };

            var view = new ValidationErrorView
            {
                DataContext = viewModel
            };

            view.ShowDialog();
        }

        private static string FormatErrors(ValidationException ex)
        {
            var stringBuilder = new StringBuilder();
            foreach (var error in ex.Errors)
            {
                stringBuilder.AppendLine($"{error.Key}: {string.Join(", ", error.Value)}");
            }
            return stringBuilder.ToString();
        }

    }
}
