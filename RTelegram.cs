using System.Collections.Generic;
using System.Net.Security;
using System.Text;
using Microsoft.Win32.SafeHandles;
using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Payments;
using Telegram.Bot.Types.ReplyMarkups;
using TusiaBot;
using File = System.IO.File;

namespace GalleryBot
{
    public enum Status
    {
        None,
        Payed
    }
    public enum Role {
        Standart,
        Admin
    }
    public class RTelegeram
    {
        public static List<string>? _Messages { get; set; } = new List<string> { 
            "1. Приветственный текст",
            "2. Меню",
            "3. Ближайшая тусовка",
            "4. Присылай сюда лавандос +798773**32 \nПришлите чек об оплате...",
            "5. ",
            ""

        };

        public static List<Person> _Persons;
        public static ITelegramBotClient bot = new TelegramBotClient("6454247073:AAHpQgLwxdBqOw3dn-DCwSRF4YwN4wXWRbs");
        
        public static void Start()
         {
             _Persons = GetPersons();
            var cts = new CancellationTokenSource();
            var cancellationToken = cts.Token;
            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = { },
            };

            bot.StartReceiving(HandleUpdateAsync, HandleErrorAsync, receiverOptions, cancellationToken);
        }

        private static List<string>? GetMessages()
        {
            string path = Environment.CurrentDirectory + @"/messages.json"; if (!File.Exists(path)) File.Create(path);
            var jsString = File.ReadAllText(path);
            var messages = JsonConvert.DeserializeObject<List<string>>(jsString) ?? null;
            return messages;
        }

        private static List<Person> GetPersons()
        {
            return Person.GetAll();
        }

        private static void AddPerson(Person person)
        {
            var flag = true;
            foreach (var person1 in _Persons)
            {
                if(person1.ChatId == person.ChatId)
                    flag = false;
            }
            if (flag)
                _Persons.Add(person);

            Person.SetAll(_Persons);
        }


        private static InlineKeyboardMarkup InlineAdmin(int num)
        {
            InlineKeyboardMarkup inlineAdmin =  
                new InlineKeyboardMarkup(new[] { InlineKeyboardButton.WithCallbackData("Изм.текст",num.ToString()) });
            return inlineAdmin;
        }
        public static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            ReplyKeyboardMarkup keyboard1 = new ReplyKeyboardMarkup(new[] { new KeyboardButton[] { "Тусовка", "Оплатить" } }) { ResizeKeyboard = true };
            
            
            ReplyKeyboardMarkup keyboard2 = new ReplyKeyboardMarkup(new[] { new KeyboardButton[] { "Тусовка", "Оплатить" }, 
                new KeyboardButton[] { "Помощь", "Статус" } }) { ResizeKeyboard = true };
            ReplyKeyboardMarkup keyboard3 = new ReplyKeyboardMarkup(new[] { new KeyboardButton[] { "Отмена" } }) { ResizeKeyboard = true };
            ReplyKeyboardMarkup keyboard4 = new ReplyKeyboardMarkup(new[] { new KeyboardButton[] { "Помощь", "Статус", "Меню" } }) { ResizeKeyboard = true };





            try
            {
                string message = update.Message.Text;
                var photo = update.Message.Photo;


                ChatId chatId = new ChatId(update.Message.Chat.Id);
                var personMain = Person.GetById(chatId, _Persons);

                if (update.Message.Document != null)
                {
                    await bot.SendTextMessageAsync(chatId,
                        "Привет, мы конечно могли бы скачать, открыть и проверить этот файл, " +
                        "но если там чек об оплате, то просто отправь его как фото");

                }

                if (photo != null)
                {
                    var fileId = photo.First().FileId;
                    InputFile inputFile = InputFile.FromFileId(fileId);

                    foreach (var person in _Persons)
                    {
                        if (person.Role == Role.Admin)
                            await bot.SendPhotoAsync(person.ChatId, inputFile);
                    }
                    
                    await bot.SendTextMessageAsync(chatId, "Твоя жопотерка отослана админам, жди новостей лол", replyMarkup: keyboard4);
                    personMain.Status = Status.Payed;
                }

                if(message == null) return;

                if (message.ToLower() == "/start")
                {
                    var inlineAdmin = InlineAdmin(1);
                    await bot.SendTextMessageAsync(chatId, _Messages[0]);
                    await bot.SendTextMessageAsync(chatId, _Messages[1], replyMarkup: keyboard1);

                    if (chatId == 6375432333 && chatId == 844052107)
                    {
                        AddPerson(new Person { ChatId = chatId, Status = Status.None, Role = Role.Admin }); return;
                    }


                    AddPerson(new Person { ChatId = chatId, Status = Status.None, Role = Role.Standart }); return;
                }
                if (message.ToLower() == "отмена")
                {
                    await bot.SendTextMessageAsync(chatId, _Messages[1], replyMarkup: keyboard1);
                    await bot.DeleteMessageAsync(chatId, update.Message.MessageId);
                    await bot.DeleteMessageAsync(chatId, update.Message.MessageId - 1); return;
                }
                if (message.ToLower() == "меню")
                {
                    await bot.SendTextMessageAsync(chatId, _Messages[1], replyMarkup: keyboard1);
                    await bot.DeleteMessageAsync(chatId, update.Message.MessageId);
                    await bot.DeleteMessageAsync(chatId, update.Message.MessageId - 1); return;



                }
                if (message.ToLower() == "тусовка")
                {
                    await bot.SendTextMessageAsync(chatId, _Messages[2], replyMarkup: keyboard1);
                    await bot.DeleteMessageAsync(chatId, update.Message.MessageId);
                    await bot.DeleteMessageAsync(chatId, update.Message.MessageId - 1); return;
                }
                if (message.ToLower() == "оплатить" && personMain.Status != Status.Payed)
                {
                    await bot.SendTextMessageAsync(chatId, _Messages[3], replyMarkup: keyboard3);
                    await bot.DeleteMessageAsync(chatId, update.Message.MessageId);
                    await bot.DeleteMessageAsync(chatId, update.Message.MessageId - 1); return;
                }
                else
                {
                    if (personMain.Status == Status.Payed)
                    {
                        await bot.SendTextMessageAsync(personMain.ChatId,
                            "Братишка, не кипишуй за деревянные, скоро поглядим");
                    }
                }
                if (message.ToLower() == "статус")
                {
                    await bot.SendTextMessageAsync(chatId, "Твой статус: свинота!", replyMarkup: keyboard2);
                    await bot.DeleteMessageAsync(chatId, update.Message.MessageId);
                    await bot.DeleteMessageAsync(chatId, update.Message.MessageId - 1); return;
                }
                if (message.ToLower() == "помощь")
                {
                    await bot.SendTextMessageAsync(chatId, "Перерыв на обед 10 мин, ожидайте...", replyMarkup: keyboard2);
                    await bot.DeleteMessageAsync(chatId, update.Message.MessageId);
                    await bot.DeleteMessageAsync(chatId, update.Message.MessageId - 1);
                    return;
                }
            }
            catch (Exception ex)
            {

            }
        }
        public static async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            await Task.Run(() => { });
        }
    }

    public class Person
    {

        public Status Status { get; set; }
        public Role Role { get; set; }
        public ChatId? ChatId { get; set; }
        public static List<Person> GetAll()
        {
            
            var path = Environment.CurrentDirectory + @"\persons.json";
            if (!File.Exists(path)) File.Create(path);

            using var reader = new StreamReader(path);
            var jsonString = reader.ReadToEnd();
            reader.Close();
            
            return JsonConvert.DeserializeObject<List<Person>>(jsonString) ?? new List<Person>();
        }
                
        public static void SetAll(List<Person> listPersons)
        {
            var path = Environment.CurrentDirectory + @"\persons.json";
            var jsonString = JsonConvert.SerializeObject(listPersons);
            var buffer = Encoding.Default.GetBytes(jsonString);

            using FileStream fs = new FileStream(path, FileMode.Create);

            fs.WriteAsync(buffer, 0, buffer.Length);
            fs.Close();
        }

        public static Person? GetById(ChatId chatId, List<Person> list)
        {
            foreach (var person in list)
            {
                if(person.ChatId == chatId) return person;
            }

            return null;
        }
    }
}