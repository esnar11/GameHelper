using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using GameHelper.Utils;
using Microsoft.Win32;

namespace GameHelper.Windows
{
    public partial class CreateNameWindow
    {
        private CancellationTokenSource _cancellationTokenSource;

        public CreateNameWindow()
        {
            InitializeComponent();

            _tbRuWords.Text = string.Join(Environment.NewLine,
                "Нуб",
                "Танк",
                "Рысь");
            _tbLetters.Text = string.Join(Environment.NewLine,
                "А -> A",
                "В -> B",
                "Е -> E",
                "З -> 3",
                "К -> K",
                "М -> M",
                "Н -> H",
                "О -> O",
                "Р -> P",
                "С -> C",
                "Т -> T",
                "Х -> X",
                "а -> a",
                "е -> e",
                "о -> o",
                "р -> p",
                "с -> c",
                "у -> y",
                "х -> x",
                "Ы -> bI",
                "Ь -> b");

            TuneControls();
        }

        private void TuneControls()
        {
            _cbCaps.IsEnabled = _cancellationTokenSource == null;
            _btnStart.IsEnabled = _cancellationTokenSource == null;
            _btnStop.IsEnabled = _cancellationTokenSource != null;

            _tbRuWords.IsEnabled = _cancellationTokenSource == null;
            _tbLetters.IsEnabled = _cancellationTokenSource == null;

            _btnSortByName.IsEnabled = _cancellationTokenSource == null;
            _btnSortByLength.IsEnabled = _cancellationTokenSource == null;
        }

        private void OnStartClick(object sender, RoutedEventArgs e)
        {
            try
            {
                _tbResult.Text = string.Empty;

                _cancellationTokenSource = new CancellationTokenSource();

                var searchContext = new SearchContext
                {
                    CancellationToken = _cancellationTokenSource.Token,
                    RussianWords = _tbRuWords.Text.Split(Environment.NewLine),
                    Letters = _tbLetters.Text
                        .Split(Environment.NewLine)
                        .Where(s => !string.IsNullOrWhiteSpace(s))
                        .Select(NameCreator.Letter.Parse)
                        .ToArray(),
                    CheckUpperCase = _cbCaps.IsChecked == true
                };
                searchContext.OnSearched += SearchContext_OnSearched;
                searchContext.Completed += SearchContext_Completed;
                ThreadPool.QueueUserWorkItem(Work, searchContext);

                TuneControls();
            }
            catch (Exception exception)
            {
                App.ShowError(exception);
            }
        }

        private void SearchContext_Completed()
        {
            this.Do(() =>
            {
                _cancellationTokenSource = null;
                TuneControls();
            });
        }

        private void SearchContext_OnSearched(string enWord)
        {
            this.Do(() =>
            {
                _tbResult.Text += enWord + Environment.NewLine;
            });
        }

        private static void Work(object? state)
        {
            var searchContext = (SearchContext) state;

            foreach (var russianWord in searchContext.RussianWords)
            {
                if (searchContext.CancellationToken.IsCancellationRequested)
                    break;
                
                if (searchContext.CheckUpperCase)
                {
                    var enWORD = NameCreator.CreateWord(russianWord.ToUpper(), searchContext.Letters);
                    if (!string.IsNullOrEmpty(enWORD))
                        searchContext.Add(enWORD);
                }
                else
                {
                    var enWord = NameCreator.CreateWord(russianWord, searchContext.Letters);
                    if (!string.IsNullOrEmpty(enWord))
                        searchContext.Add(enWord);
                }
            }

            searchContext.RaiseComplete();
        }

        private void OnStopClick(object sender, RoutedEventArgs e)
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource = null;
            TuneControls();
        }

        public class SearchContext
        {
            private readonly ICollection<string> _result = new List<string>();

            public CancellationToken CancellationToken { get; set; }

            public IReadOnlyCollection<string> RussianWords { get; set; }

            public IReadOnlyCollection<NameCreator.Letter> Letters { get; set; }

            public bool CheckUpperCase { get; set; }

            public event Action<string> OnSearched;

            public event Action Completed;

            public IReadOnlyCollection<string> Result => _result.ToArray();

            public void Add(string enWord)
            {
                _result.Add(enWord);
                OnSearched?.Invoke(enWord);
            }

            public void RaiseComplete()
            {
                Completed?.Invoke();
            }
        }

        private void OnParseClick(object sender, RoutedEventArgs e)
        {
            var fileDialog = new OpenFileDialog { Multiselect = true };
            if (fileDialog.ShowDialog() == true)
                try
                {
                    Cursor = Cursors.Wait;

                    var count = 0;

                    foreach (var fileName in fileDialog.FileNames)
                    {
                        using var file = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
                        using var reader = new StreamReader(file);
                        var text = reader.ReadToEnd();
                        count += Parse(text);
                    }

                    MessageBox.Show($"Добавлено слов: {count}");
                }
                catch (Exception exc)
                {
                    App.ShowError(exc);
                }
                finally
                {
                    Cursor = null;
                }
        }

        private int Parse(string text)
        {
            var list = new List<string>();
            list.AddRange(_tbRuWords.Text.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries));

            var oldCount = list.Count;

            foreach (var line in text.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries))
            foreach (var w in line.Split(" ", StringSplitOptions.RemoveEmptyEntries))
                if (w.Length > 2 && w.All(NameCreator.Letter.IsRu))
                {
                    var word = char.ToUpper(w[0]) + w.Substring(1);
                    if (!list.Contains(word))
                        list.Add(word);
                }

            _tbRuWords.Text = string.Join(Environment.NewLine, list.OrderBy(s => s));

            return list.Count - oldCount;
        }

        private void OnSortByNameClick(object sender, RoutedEventArgs e)
        {
            var words = _tbResult.Text.Split(Environment.NewLine);
            _tbResult.Text = string.Join(Environment.NewLine, words.Where(w => !string.IsNullOrWhiteSpace(w)).OrderBy(w => w));
        }

        private void OnSortByLengthClick(object sender, RoutedEventArgs e)
        {
            var words = _tbResult.Text.Split(Environment.NewLine);
            _tbResult.Text = string.Join(Environment.NewLine, words.Where(w => !string.IsNullOrWhiteSpace(w)).OrderByDescending(w => w.Length));
        }
    }
}
