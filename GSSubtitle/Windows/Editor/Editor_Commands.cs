using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace GSSubtitle.Windows.Editor
{
    public static class Editor_Commands
    {
        private static RoutedUICommand _Marge = new RoutedUICommand("Marge", "MargeCommad", typeof(Editor_Commands));
        private static RoutedUICommand _NormalText = new RoutedUICommand("Normal", "NormalTextCommand", typeof(Editor_Commands));
        private static RoutedUICommand _AddLine = new RoutedUICommand("Add Line", "AddLineCommand", typeof(Editor_Commands));
        private static RoutedUICommand _InsetLine = new RoutedUICommand("Insert Line", "InsertLineCommand", typeof(Editor_Commands));
        private static RoutedUICommand _InsetLineBefore = new RoutedUICommand("Inset Line Before This Line", "InsertBeforeCommand", typeof(Editor_Commands));
        private static RoutedUICommand _InsertLineAfter = new RoutedUICommand("Inset Line After This Line", "InsetLineAfterCommand", typeof(Editor_Commands));
        private static RoutedUICommand _DeleteLines = new RoutedUICommand("Delete Line(s)", "DleteLineCommand", typeof(Editor_Commands));
        private static RoutedUICommand _TextMenuItemCommand = new RoutedUICommand("Text", "TextMenuItemCommand", typeof(Editor_Commands));
        private static RoutedUICommand _ClearTextCommand = new RoutedUICommand("Clear Text", "ClearTextCommand", typeof(Editor_Commands));
        private static RoutedUICommand _ClearOriginalTextCommand = new RoutedUICommand("Clear Original Text", "ClearOriginalTextCommand", typeof(Editor_Commands));
        private static RoutedUICommand _CopyTextToClipboardCommand = new RoutedUICommand("Copy Text To Clipboard", "CopyTextToClipboardCommand", typeof(Editor_Commands));
        private static RoutedUICommand _PasteFromClipboardCommand = new RoutedUICommand("Past Text From Clipboard", "PasteFromClipboardCommand", typeof(Editor_Commands));
        private static RoutedUICommand _SetTextAsOriginalTextCommand = new RoutedUICommand("Set Text As Original Text", "SetTextAsOriginalTextCommand", typeof(Editor_Commands));
        private static RoutedUICommand _SetOriginalTextAsTextCommand = new RoutedUICommand("Set Original Text As Text", "SetOriginalTextAsTextCommand", typeof(Editor_Commands));
        private static RoutedUICommand _SyncOriginalTextAndText = new RoutedUICommand("Text ↔ Original Text", "SyncOriginalTextAndText", typeof(Editor_Commands));
        private static RoutedUICommand _SplitSelectedLinesCommand = new RoutedUICommand("Split Selected Lines", "SplitSelectedLinesCommand", typeof(Editor_Commands));
        private static RoutedUICommand _BoldTextCommand = new RoutedUICommand("Bold", "BoldTexCommand", typeof(Editor_Commands));
        private static RoutedUICommand _ItalicTextCommand = new RoutedUICommand("Italic","ItalicTextCommand",typeof(Editor_Commands));
        private static RoutedUICommand _UndoCommand = new RoutedUICommand("Undo", "UndoCommand", typeof(Editor_Commands));
        private static RoutedUICommand _RedoCommand = new RoutedUICommand("Redo", "RedoCommand", typeof(Editor_Commands));
        private static RoutedUICommand _SurroundWithTagCommand = new RoutedUICommand("Tag", "SurroundWithTagCommand", typeof(Editor_Commands));
        private static RoutedUICommand _AddSpecialCharacterCommand = new RoutedUICommand("Special Character", "AddSpecialCharacterCommand", typeof(Editor_Commands));
        private static RoutedUICommand _AddControlCharacterCommand = new RoutedUICommand("Control Character", "AddControlCharacterCommand", typeof(Editor_Commands));
        private static RoutedUICommand _SurroundWithSpecialCharCommand = new RoutedUICommand("Surround With", "SourroundWithSpecialCharCommand", typeof(Editor_Commands));
        private static RoutedUICommand _SubtitleAlignmentCommand = new RoutedUICommand("Alignment", "SubtitleAlignmentCommand", typeof(Editor_Commands));
        private static RoutedUICommand _KarokeEffectCommand = new RoutedUICommand("Karoake", "KaroakeEffectCommand", typeof(Editor_Commands));

        public static RoutedUICommand Marge { get { return _Marge; } }
        public static RoutedUICommand NormalText { get { return _NormalText; } }
        public static RoutedUICommand AddLine { get { return _AddLine; } }
        public static RoutedUICommand InsertLine { get { return _InsetLine; } }
        public static RoutedUICommand InsetLineBeforeCurrentLine { get { return _InsetLineBefore; } }
        public static RoutedUICommand InsetLineAfterCurrentLine { get { return _InsertLineAfter; } }
        public static RoutedUICommand DeleteLines { get { return _DeleteLines; } }
        public static RoutedUICommand TextMenuItemCommand { get { return _TextMenuItemCommand; } }
        public static RoutedUICommand ClearTextCommand { get { return _ClearTextCommand; } }
        public static RoutedUICommand CLearOriginalTextCommand { get { return _ClearOriginalTextCommand; } }
        public static RoutedUICommand CopyTextToClipboard { get { return _CopyTextToClipboardCommand; } }
        public static RoutedUICommand PasteTextFromClipboardCommand { get { return _PasteFromClipboardCommand; } }
        public static RoutedUICommand SetTextAsOriginalText { get { return _SetTextAsOriginalTextCommand; } }
        public static RoutedUICommand SetOriginalTextAsTextCommand { get { return _SetOriginalTextAsTextCommand; } }
        public static RoutedUICommand SyncOriginalTextAndText { get { return _SyncOriginalTextAndText; } }
        public static RoutedUICommand SplitLSelectedLinesCommand { get { return _SplitSelectedLinesCommand; } }
        public static RoutedUICommand BoldTextCommand { get { return _BoldTextCommand; } }
        public static RoutedUICommand ItalicTextCommand { get { return _ItalicTextCommand; } }
        public static RoutedUICommand UndoCommand { get { return _UndoCommand; } }
        public static RoutedUICommand RedoCommand { get { return _RedoCommand; } }
        public static RoutedUICommand SurroundWithTagCommand { get { return _SurroundWithTagCommand; } }
        public static RoutedUICommand AddSpecialCharacter { get { return _AddSpecialCharacterCommand; } }
        public static RoutedUICommand AddControlCharacterCommand { get { return _AddControlCharacterCommand; } }
        public static RoutedUICommand SurroundWithSpecialCharCommand { get { return _SurroundWithSpecialCharCommand; } }
        public static RoutedUICommand SubtitleAlignmentCommand { get { return _SubtitleAlignmentCommand; } }
public static RoutedUICommand KaroakeEffectCommand { get { return _KarokeEffectCommand; } }
    }
}
