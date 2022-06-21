using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace KompasCore.Cards.Parser
{
    public class TextParser : MonoBehaviour
    {
        public class Line
        {
            public int Indentation { get; }
            public string[] Tokens { get; }

            public string FirstToken => Tokens.FirstOrDefault();

            public Line(string text)
            {
                foreach (char c in text.ToCharArray())
                {
                    if (c == '\t') Indentation++;
                    else break;
                }
                Tokens = text.Trim().Split(':');
            }
        }

        public class Block
        {
            public Block Parent { get; }
            public Line Line { get; }
            public List<Block> Elements { get; }

            public Block(Block parent, Line line)
            {
                Parent = parent;
                Line = line;
                Elements = new List<Block>();
            }
        }

        public Block Parse(string str)
        {
            int indentation = 0;
            string[] lines = str.Split('\n');

            Block currentBlock = new Block(default, default);
            Block lastSeenBlock = currentBlock;

            foreach (var line in lines)
            {
                Line currentLine = new Line(line);
                if (currentLine.Indentation < indentation)
                {
                    currentBlock = currentBlock.Parent;
                }
                else if (currentLine.Indentation > indentation)
                {
                    currentBlock = lastSeenBlock;
                }

                lastSeenBlock = new Block(currentBlock, currentLine);
                currentBlock.Elements.Add(lastSeenBlock);
            }

            while (currentBlock.Parent != null) currentBlock = currentBlock.Parent;

            return currentBlock;
        }
    }
}