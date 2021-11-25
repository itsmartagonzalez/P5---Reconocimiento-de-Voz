using System;
// using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Windows.Speech;
using System.Linq;

public class Voice : MonoBehaviour {

    KeywordRecognizer keywordRecognizer;
    Dictionary<string, System.Action> keywords = new Dictionary<string, System.Action>();

    // Start is called before the first frame update
    void Start() {
        keywordRecognizer = new KeywordRecognizer(keywords.Keys.ToArray());
        keywordRecognizer.OnPhraseRecognized += KeywordOnPhraseRecognized;
        keywordRecognizer.Start();    
    }

    void addKeywords() {
        keywords.Add("hola", () => {
            Debug.Log("hola");
        });
    }

    void KeywordOnPhraseRecognized(PhraseRecognizedEventArgs args) {
        System.Action keywordAction;

        if (keywords.TryGetValue(args.text, out keywordAction)) {
            keywordAction.Invoke();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
