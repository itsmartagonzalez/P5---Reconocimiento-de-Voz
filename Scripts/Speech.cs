using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using UnityEngine.Windows.Speech;
using UnityEngine.UI;

public class Roomba : MonoBehaviour {

    private bool start = false;
    private Vector3 desired;
    private KeywordRecognizer keywordRecognizer;
    private Dictionary<string, Action> actions = new Dictionary<string, Action>();
    private Text tv;
    Rigidbody rb;

    private DictationRecognizer dictationRecognizer;
    // Start is called before the first frame update
    void Start() {
        rb = GetComponent<Rigidbody>();
        tv = GameObject.Find("TV_Text").GetComponent<Text>();
        startKeyword();
        startDictate();
    }

    void startKeyword() {
        actions.Add("delante", Forward);
        actions.Add("atras", Back);
        actions.Add("derecha", Right);
        actions.Add("izquierda", Left);
        actions.Add("para", Stop);
        actions.Add("friega", Friega);
        actions.Add("escribe", Dictate);
        keywordRecognizer = new KeywordRecognizer(actions.Keys.ToArray());
        keywordRecognizer.OnPhraseRecognized += Functionality;
        keywordRecognizer.Start();
    }

    void startDictate() {
        dictationRecognizer = new DictationRecognizer();
        dictationRecognizer.InitialSilenceTimeoutSeconds = 5;
		dictationRecognizer.AutoSilenceTimeoutSeconds = 2;
		dictationRecognizer.DictationResult += DictationResult;
		dictationRecognizer.DictationComplete += DictationComplete;
    }

    void FixedUpdate() {
        if (start) {
            Invoke("movePosition", 0);
        }
    }

    private void Functionality(PhraseRecognizedEventArgs speech) {
        Debug.Log(speech.text);
        actions[speech.text].Invoke();
    }

    private void Dictate() {
        PhraseRecognitionSystem.Shutdown();
        dictationRecognizer.Start();
        tv.text = "Te estoy escuchando...";
    }

    private void DictationResult(string text, ConfidenceLevel confidence) {
        tv.text = text;
	}

    private void DictationComplete(DictationCompletionCause cause) {
        dictationRecognizer.Stop ();
		PhraseRecognitionSystem.Restart(); 
	}

    private void Forward() {
        start = true;
        desired = transform.forward;
        Invoke("movePosition", 0);
    }

    private void Right() {
        start = true;
        Turn(90);
    }

    private void Left() {
        start = true;
        Turn(-90);
    }

    private void Back() {
        start = true;
        Turn(180);
    }

    private void Stop() {
        start = false;
    }

    private void Friega() {
        GameObject[] drops = GameObject.FindGameObjectsWithTag("drop");
        foreach (GameObject d in drops) {
            if (Vector3.Distance(d.GetComponent<Transform>().position, transform.position) < 0.5) {
                d.SetActive(false);
                break;
            }
        }
    }

    private void movePosition() {
        rb.MovePosition(rb.position - desired.normalized * 0.5f * Time.fixedDeltaTime);
    }

    private void Turn(int angle) {
        transform.Rotate(new Vector3(0, angle, 0));
        desired = transform.forward;
    }

    void OnDestroy() {
		dictationRecognizer.DictationResult -= DictationResult;
		dictationRecognizer.DictationComplete -= DictationComplete;
		dictationRecognizer.Dispose();  //creo que no es necesario explicar pero es necesario que vaya todo esto.. 
	}
}
