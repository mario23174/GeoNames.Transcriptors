using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using GeoNames.Transcriptors;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GeoNames.Tanscriptors.Tests
{
    [TestClass]
    public class TranscriptorsTests
    {
        #region GeoTools Tests

        [TestMethod]
        public void TransliterateLitTest()
        {
            var result = new Dictionary<string, string>();
            var initialList = new Dictionary<string, string>
            {
                {"Akmenė", "Акмяне"},
                {"Anykščiai", "Аникщяй"},
                {"Baltoji Vokė", "Балтойи-Воке"},
                {"Vabalninkas", "Вабальнинкас"},
                {"Visaginas", "Висагинас"},
                {"Gelgaudiškis", "Гялгаудушкис"},
                {"Žiežmariai", "Жежмаряй"},
                {"Joniškėlis", "Йонишкелис"},
                {"Kazlų Rūda", "Казлу Руда"},
                {"Kaišiadorys", "Кайшядорис"},
                {"Marijampolė", "Мариямполе"},
                {"Naujoji Akmenė", "Науйойи-Акмяне"},
                {"Panevėžys", "Панявежис"},
                {"Švenčionėliai", "Швенчёнеляй"},
                {"Švenčionys", "Швянчёнис"},
                {"Eišiškės", "Эйшишкес"},
                {"Pjūkla", "Пьюкла"},
            };

            var trans = new LithuaniaTranscriptor();
            foreach (var pair in initialList)
            {
                result.Add(pair.Value, $@" transed: {trans.ToRussian(pair.Key)}");
            }
            Assert.IsTrue(result.Any());
        }

        [TestMethod]
        public void TransliterateLatTest()
        {
            var result = new Dictionary<string, string>();
            var initialList = new Dictionary<string, string>
            {
                {"Aizkraukle", "Айзкраукле"},
                {"Aizpute", "Айзпуте"},
                {"Alūksne", "Алуксне"},
                {"Auce", "Ауце"},
                {"Valdemārpils", "Валдемарпилс"},
                {"Viļāni", "Виляны"},
                {"Jēkabpils", "Екабпилс"},
                {"Saulkrasti", "Саулкрасты"},
                {"Jaunjelgava", "Яунъелгава"},
                {"Jūrmala", "Юрмала"},
                {"Cēsis", "Цесис"},
                {"Strenči", "Стренчи"},
                {"Salacgrīva", "Салацгрива"},
                {"Rūjiena", "Руйиена"},
                {"Liepāja", "Лиепая"},
                {"Lielvārde", "Лиелварде"},
                {"Līgatne", "Лигатне"},
            };

            var trans = new LatviaTranscriptor();
            foreach (var pair in initialList)
            {
                result.Add(pair.Value, $@" transed: {trans.ToRussian(pair.Key)}");
            }
            Assert.IsTrue(result.Any());
        }

        [TestMethod]
        public void TransliteratePolishTest()
        {
            var result = new Dictionary<string, string>();
            var initialList = new Dictionary<string, string>
            {
                {"Warszawa", "Варшава"},
                {"Kraków", "Краков"},
                {"Łódź", "Лодзь"},
                {"Wrocław", "Вроцлав"},
                {"Szczecin", "Щецин"},
                {"Bydgoszcz", "Быдгощ"},
                {"Częstochowa", "Ченстохова"},
                {"Toruń", "Торунь"},
                {"Rzeszów", "Жешув"},
                {"Bielsko-Biała", "Бельско-Бяла"},
                {"Dąbrowa Górnicza", "Домброва-Гурнича"},
                {"Włocławek", "Влоцлавек"},
                {"Jastrzębie-Zdrój", "Ястшембе-Здруй"},
                {"Nowy Sącz", "Новы-Сонч"},
                {"Chorzów", "Хожув"},
                {"Kalisz", "Калиш"},

            };

            var trans = new PolandTranscriptor();
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            foreach (var pair in initialList)
            {
                result.Add(pair.Value, $@" transed: {trans.ToRussian(pair.Key)}");
            }
            stopwatch.Stop();

            Console.WriteLine(stopwatch.ElapsedMilliseconds);

            Assert.IsTrue(result.Any());
        }


        [TestMethod]
        public void TransliterateEstonianTest()
        {
            var result = new Dictionary<string, string>();
            var initialList = new Dictionary<string, string>
            {
                {"äksi", "экси"},
                {"köörna", "кёэрна"},
                {"Hiiemetsa", "Хийеметса"},
                {"Mõisaküla", "Мыйзакюла"},
                {"Õismäe", "Ыйсмяэ"},
                {"Jõõpre", "Йыыпре"},
                {"Ääsmäe", "Ээсмяэ"},
                {"Kohtla-Järve", "Кохтла-Ярве"},
                {"Sillamäe", "Силламяэ"},
                {"Haapsalu", "Хаапсалу"},
                {"Jõgeva", "Йыгева"},
                {"Põltsamaa", "Пылтсамаа"},
                {"Narva-Jõesuu", "Нарва-Йыэсуу"},
                {"Kilingi-Nõmme", "Килинги-Нымме"},
            };

            var trans = new EstoniaTranscriptor();
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            foreach (var pair in initialList)
            {
                result.Add(pair.Value, $@" transed: {trans.ToRussian(pair.Key)}");
            }
            stopwatch.Stop();

            Console.WriteLine(stopwatch.ElapsedMilliseconds);

            Assert.IsTrue(result.Any());
        }
        #endregion
    }
}
