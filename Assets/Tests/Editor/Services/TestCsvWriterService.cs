using System;
using System.IO;
using Models.LogEntry;
using NUnit.Framework;
using Services;
using UnityEngine;

namespace Tests.Editor.Services
{
    [TestFixture]
    public class TestCsvWriterService
    {
        private readonly string _filename = $"{DateTime.UtcNow:yyyyMMddHHmmss}.csv";
        private readonly string _logDirectory = $"{Application.dataPath}/Log/Test/";
        
        [Test]
        public void TestCreatesLogDirectory()
        {
            var c = new CsvWriterService();
            c.CreateFile(_logDirectory, _filename);
            
            Assert.That(Directory.Exists(_logDirectory));
            CleanUp(_logDirectory);
        }

        [Test]
        public void TestCreatesLogFile()
        {
            var c = new CsvWriterService();
            c.CreateFile(_logDirectory, _filename);
            var files = Directory.GetFiles(_logDirectory);
            
            if (files.Length <= 0) Assert.Fail();
            else Assert.That(string.Equals(files[0], $"{_logDirectory}{_filename}"));
            CleanUp(_logDirectory);
        }

        [Test]
        public void TestCanWriteLogEntryToFile()
        {
            var c = new CsvWriterService();
            c.CreateFile(_logDirectory, _filename);
            c.WriteLogEntry(new SimpleLogEntry("test", Directions.Inbound, LogEntryTypes.PingPacket));
            
            Assert.That(File.ReadAllLines($"{_logDirectory}{_filename}").Length >= 2);
            CleanUp(_logDirectory);
        }

        private static void CleanUp(string path)
        {
            var files = Directory.GetFiles(path);
            foreach (var f in files)
            {
                File.SetAttributes(f, FileAttributes.Normal);
                File.Delete(f);
            }
            Directory.Delete(path);
        }
    }
}