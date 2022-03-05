﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class Consts
{
    // Paths
    internal const string MenuName = "Utility Ai System/";
    internal const string Path_MainFolder = "Assets/UAS/";
    internal const string Path_ScriptableObjects = Path_MainFolder + "ScriptableObjects/";
    internal const string Path_Templates = Path_ScriptableObjects + "Templates/Templates.asset";
    internal const string Path_AiTickerSettings = Path_MainFolder + "Settings/AiTickerSettings" + "." + FileExtension_TickerSettings;
    internal const string Path_PlayAi = Path_MainFolder + "Ai/Playable" + "." + FileExtension_UasTemplateCollection;

    internal const string Path_AiAsset = Path_ScriptableObjects + "Ais/";
    internal const string Path_BucketAsset = Path_ScriptableObjects + "Buckets/";
    internal const string Path_DecisionAsset = Path_ScriptableObjects + "Decisions/";
    internal const string Path_ConsiderationAsset = Path_ScriptableObjects + "Considerations/";
    internal const string Path_UASManagerModel = Path_ScriptableObjects + "Models/Manager/UASMModel.asset";

    internal const string File_MainSavePath = Path_MainFolder + "Persistence/";
    internal const string File_UASModel = File_MainSavePath + "UASTemplates.dat";
    internal const string FileName_UASModelJson = "UASTemplates.json";

    // Labels
    internal const string Label_MainWindowModel = "MainWindowModel";
    internal const string Label_AgentActionModel = "AgentActions";
    internal const string Label_ConsiderationModel = "Considerations";
    internal const string Label_DecisionModel = "Decisions";
    internal const string Label_BucketModel = "Buckets";
    internal const string Label_UAIModel = "AIs";


    // Scorers
    public const string Default_UtilityScorer = Name_USAverageScore;
    public const string Default_BucketSelector = Name_UCSHighestScore;
    public const string Default_DecisionSelector = Name_UCSHighestScore;

    public const string Name_UCSHighestScore = "Highest Score";
    public const string Description_UCSHighestScore = "Selects the highest scored object";

    public const string Name_USAverageScore = "Average";
    public const string Description_USAverageScore = "Returns the average score of all considerations. Returns 0 if one consideration is 0";

    public const string Name_USCompensationScorer = "Compensation";
    public const string Description_USCompensationScorer = "Returns all considerations scores multiplied and compensates for multiplying by <1.";

    public const string Name_DefaultDSE = "Default DSE";
    public const string Description_DefaultDSE = "Selects the highest scored object";

    // Editor Windows
    public const string Name_TemplateManager = "Template Manager";
    public const string Name_AiInspector = "Runtime Inspector";
    public const string Name_AiTickerManager = "Ticker Manager";


    // Ticker Modes
    public const string Description_TickerModeUnrestricted = "No restrictions to execution time";
    public const string Description_TickerModeDesiredFrameRate = "Dynamically alters ticks/frame to stay above target framerate. Increasing samplerate trades Reaction time for framerate precision.";
    public const string Description_TickerModeTimeBudget = "Executes as many ticks as possible in a given timeframe. The Time budget may be exceeded by the execution time of one agent";


    // File extensions
    public const string FileExtension_AgentAction = "action";
    public const string FileExtension_Consideration = "consideration";
    public const string FileExtension_Decision = "decision";
    public const string FileExtension_Bucket = "bucket";
    public const string FileExtension_UAI = "uai";
    public const string FileExtension_UasTemplateCollection = "uaiproj";
    public const string FileExtension_TickerSettings = "setting";
    public const string FileExtension_RestoreAbleCollection = "collection";

    public static readonly string[] FileExtensions =
    {
        "AgentAction", FileExtension_AgentAction,
        "Consideration", FileExtension_Consideration,
        "Decision", FileExtension_Decision,
        "Bucket", FileExtension_Bucket,
        "AI", FileExtension_UAI,
        "Templates", FileExtension_UasTemplateCollection,
        "Ticker Settings", FileExtension_TickerSettings,
    };


}