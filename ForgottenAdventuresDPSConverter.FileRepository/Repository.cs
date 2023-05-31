using ForgottenAdventuresDPSConverter.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ForgottenAdventuresDPSConverter.FileRepository
{
    public abstract class Repository<T> : IRepository<T> where T : class
    {
        private static readonly Dictionary<string, Queue<int>> writingQueuePerFile = new();
        private static readonly Dictionary<string, int> nextQueueNumberPerFile = new();
        private static readonly Dictionary<string, TaskCompletionSource<bool>> queueCompletionPerFile = new();

        protected static int GetWritingQueueNumber(string filePath)
        {
            if(writingQueuePerFile.ContainsKey(filePath) == false)
            {
                writingQueuePerFile.Add(filePath, new());
                nextQueueNumberPerFile.Add(filePath, 0);
                queueCompletionPerFile.Add(filePath, new());
                queueCompletionPerFile[filePath].SetResult(true);
            }

            int queueNumber = nextQueueNumberPerFile[filePath];
            nextQueueNumberPerFile[filePath]++;
            writingQueuePerFile[filePath].Enqueue(queueNumber);

            return queueNumber;
        }

        protected async static Task<int> AwaitWritingQueueNumber(string filePath)
        {
            int number = GetWritingQueueNumber(filePath);
            await AwaitWritingQueueNumber(filePath, number);
            return number;
        }

        protected async static Task AwaitWritingQueueNumber(string filePath, int queueNumber)
        {
            if(writingQueuePerFile.ContainsKey(filePath) == false)
            {
                throw new KeyNotFoundException("the filepath has not been added to the queue's Dictionary. this means that the GetQueueNumber(string) method has never been called for the file path " + filePath + " which is a requirement. an alternative is to call awaitQueueNumber(string) which await and returns the queue number. this exception indicates a problem in the code and is not a runtime error.");
            }
            else if (writingQueuePerFile[filePath].Contains(queueNumber) == false)
            {
                throw new Exception("the queue does not contain the queueNumber. this means that the number wasn't gathered by the GetQueueNumber(string) method which is a requirement as it adds the number to the queue. an alternative is to call awaitQueueNumber(string) which await and returns the queue number. this exception indicates a problem in the code and is not a runtime error.");
            }
            else
            {
                await queueCompletionPerFile[filePath].Task;
                while(writingQueuePerFile[filePath].Peek() != queueNumber)
                {
                    await queueCompletionPerFile[filePath].Task;
                }
                queueCompletionPerFile[filePath] = new(false);
                return;
            }
        }

        protected static void releaseWritingQueueNumber(string filePath, int queueNumber)
        {
            if (writingQueuePerFile.ContainsKey(filePath) == false)
            {
                throw new KeyNotFoundException("the filepath has not been added to the queue's Dictionary. this means that the GetQueueNumber(string) method has never been called for the file path " + filePath + " which is a requirement. an alternative is to call awaitQueueNumber(string) which await and returns the queue number. this exception indicates a problem in the code and is not a runtime error.");
            }
            else if (writingQueuePerFile[filePath].Contains(queueNumber) == false)
            {
                throw new Exception("the queue does not contain the queueNumber. this means that the number wasn't gathered by the GetQueueNumber(string) method which is a requirement as it adds the number to the queue. an alternative is to call awaitQueueNumber(string) which await and returns the queue number. this exception indicates a problem in the code and is not a runtime error.");
            }
            else if(writingQueuePerFile[filePath].Peek() == queueNumber)
            {
                writingQueuePerFile[filePath].Dequeue();
                queueCompletionPerFile[filePath].SetResult(true);
                return;
            }
        }

        public abstract Task<bool> Contains(Expression<Func<T, bool>> query);
        public abstract Task<int> Count(Expression<Func<T, bool>> query);
        public abstract Task<(bool, int)> Create(T toCreate);
        public abstract Task<bool> Delete(int id);
        public abstract Task<T> Get(int id);
        public abstract Task<IEnumerable<T>> GetAll();
        public abstract Task<IEnumerable<T>> GetAll(Expression<Func<T, bool>> query);
        public abstract Task<bool> Update(T toUpdate);
    }
}
