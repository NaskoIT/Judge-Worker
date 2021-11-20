using System;

namespace JudgeWorker
{
    class Program
    {
        static void Main(string[] args)
        {
            string code = @"#include <iostream>

using namespace std;

int main() {
  int a;
  int b;
  cin >> a;
  cin >> b;

  cout << a + b;
}";
            string input = "2\n3";
            var requestModel = new ExecutionRequestModel
            {
                Language_Id = 4,
                Source_Code = code,
                StdIn = input
            };


        }
    }
}
