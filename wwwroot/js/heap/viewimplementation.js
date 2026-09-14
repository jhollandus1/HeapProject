import { HashTable, fixParentReference } from './utility.js';
import { ArrayHeapNode, HeapItem, GetMode, USER_INPUT, RESTORE_FROM_DB, BULK_LOAD_METHOD, ADD_OR_REMOVE } from './arrayheap.js';
export { HeapView, getIterationValue, HeapEvent, exchangeUnderlyingValues, printLegendAndRectangles, delay }; 
// JavaScript source code

const width = 928;
const height = width;
const margin = 31;
const topX = 440;
const topY = 40;
const radius = 30;

// location of max var box
let xMaxVar = 740;
let yMaxVar = 0;
let RADIUS = 30;

// location of temp var box
let xTempVar = 740;
let yTempVar = 215;
let squareSideTempVar = 50;


/*
    const svg = d3.select("body")
        .append("svg")
        .attr("width", width)
        .attr("height", height)
        .attr("viewBox", [-margin, -margin, width, height])
        .attr("style", "width: 100%; height: auto; font: 10px sans-serif;")
        .attr("id", "mysvg");
*/

var testFeature1 = true;   // This controls the printing up to and including the Legend and 15-node heap with parent and child references (first set of features)
var testFeature2 = true;    // This controls the rest, up to where the exchangeValues function works to exchange two items in the heap

class Svg {
    static NAMESPACE_URI = 'http://www.w3.org/2000/svg';

    constructor(domElementId, thisElemId, width, height) {
        this.$svg = document.createElementNS(Svg.NAMESPACE_URI, 'svg');
        this.$svg.setAttribute('xmlns', Svg.NAMESPACE_URI);
        this.$svg.setAttribute('width', width);
        this.$svg.setAttribute('height', height);
        this.$svg.setAttribute('viewBox', `${-margin} ${-margin} ${width} ${height}`);
        this.$svg.setAttribute('id', thisElemId);
        document.querySelector(`#${domElementId}`).appendChild(this.$svg);
    }

    add(elementType, attributes = {}, styles = {}) {
        let $element = document.createElementNS(Svg.NAMESPACE_URI, elementType);

        Object.entries(attributes).forEach(([k, v]) => {
            $element.setAttribute(k, v);
        });

        Object.entries(styles).forEach(([k, v]) => {
            $element.style[k] = v;
        });

        this.$svg.appendChild($element);
        return $element;
    }
}


// constants for setting up nodes of a multi-level heap
const ht = new HashTable();
ht.set('', 1.0);
ht.set('0', 0.65);
ht.set('00', 0.50);
ht.set('01', 0.75);
ht.set('000', 0.4);
ht.set('001', 0.55);
ht.set('010', 0.70);
ht.set('011', 0.85);

ht.set('1', 1.35);
ht.set('10', 1.20);
ht.set('11', 1.45);
ht.set('100', 1.10);
ht.set('101', 1.25);
ht.set('110', 1.40);
ht.set('111', 1.55);

let svg1 = new Svg('bodyid', 'mysvg', width, height);
let svg2 = document.getElementById('mysvg');

// globalGroup1 controls the first circle, all the red circles, part of the legend, and the temporary item 
const globalGroup1 = d3.select('#mysvg')
    .append("g")
    .attr("id", "mygroup");

const groupNumbers = d3.select('#mysvg')
    .append("g")
    .attr('id', 'mygroup2')


function curvedLink(x1, y1, x2, y2, curvature = 0.5) {
    // control points for the curve
    const dx = x2 - x1;
    const dy = y2 - y1;
    const cx1 = x1 - 0.25 * dx * curvature;
    const cy1 = y1;
    const cx2 = x2 - dx * curvature;
    const cy2 = y2;
    console.log("Items for curved line: " + "dx: " + dx + " dy " + dy + " cx1 " + cx1 + " cy1 " + cy1 + " cx2 " + cx2 + " cy2 " + cy2);
    return "M" + x1 + "," + y1 + "C" + cx1 + "," + cy1 + " " + cx2 + "," + cy2 + " " + x2 + "," + y2;
}

function applyParentToChildLinesToTreeNodes(x1, y1, x2, y2, path) {
    //const line = document.createElementNS("http://www.w3.org/2000/svg", "line");
    if (testFeature1) {
        console.log("Tangent values: x1: " + x1 + " y1: " + y1 + " x2: " + x2 + " y2: " + y2);
    }
    if (testFeature2) {
        console.log(path);
    }

    let angleInRadians = Math.atan2(Math.abs(y2 - y1), Math.abs(x2 - x1));
    let angleInDegrees = Math.atan2(Math.abs(y2 - y1), Math.abs(x2 - x1)) * 180 / Math.PI;
    //let lineLengthOrig = Math.sqrt(Math.pow((y2 - y1), 2) + Math.pow((x2 - x1), 2))

    let xDiff = Math.cos(angleInRadians) * radius;
    let yDiff = Math.sin(angleInRadians) * radius;
    if (testFeature1) {
        console.log("xDiff: " + xDiff + " yDiff: " + yDiff);
    }
    if (testFeature1) {
        console.log("Angle in radians: " + angleInRadians + " angle in degrees: " + angleInDegrees);
    }
    //console.log("Line length orig:" + lineLengthOrig);

    let x1mod = -1;

    if (path.charAt(path.length - 1) == '0') {  // if path ends in 0, then it is subtracted (else added)
        x1mod = x1 - xDiff;
    }
    else {
        x1mod = x1 + xDiff;
    }

    let y1mod = y1 + yDiff; // always is positive
    let x2mod = -1;

    if (path.charAt(path.length - 1) == '0') {  // if path ends in 0, then it is added (else subtracted)
        x2mod = x2 + xDiff;
    }
    else {
        x2mod = x2 - xDiff;
    }

    let y2mod = y2 - yDiff; // always is negative

    makeChildArrowWithMarker(x1mod, y1mod, x2mod, y2mod);
}

function applyChildToParentLinesToTreeNodes(x1, y1, x2, y2, path, idNum) {
    //const line = document.createElementNS("http://www.w3.org/2000/svg", "line");
    if (testFeature1) {
        console.log("Tangent values: x1: " + x1 + " y1: " + y1 + " x2: " + x2 + " y2: " + y2);
    }

    let angleInRadians = Math.atan2(Math.abs(y2 - y1), Math.abs(x2 - x1));
    let angleInDegrees = Math.atan2(Math.abs(y2 - y1), Math.abs(x2 - x1)) * 180 / Math.PI;
    //let lineLengthOrig = Math.sqrt(Math.pow((y2 - y1), 2) + Math.pow((x2 - x1), 2))

    let xDiff = Math.cos(angleInRadians) * radius;
    let yDiff = Math.sin(angleInRadians) * radius;
    let xDiffNeg025 = Math.cos(angleInRadians - 0.25) * radius;
    let yDiffNeg025 = Math.sin(angleInRadians - 0.25) * radius;

    let xDiffPos025 = Math.cos(angleInRadians + 0.25) * radius;
    let yDiffPos025 = Math.sin(angleInRadians + 0.25) * radius;

    if (testFeature1) {
        console.log("x1: " + x1 + " y1: " + y1 + " x2: " + x2 + " y2: " + y2);
        console.log("xDiff: " + xDiff + " yDiff: " + yDiff);
        console.log("xDiff03Neg025: " + xDiffNeg025 + " yDiff03Neg025: " + yDiffNeg025);
        console.log("Angle in radians: " + angleInRadians + " (angle in radians - 0.3) for x and y component: " + (angleInRadians - 0.3));
    }
    //console.log("Line length orig:" + lineLengthOrig);

    let x1mod = -1;

    if (path.charAt(path.length - 1) == '0') {  // if path ends in 0, then it is subtracted (else added)
        x1mod = x1 - xDiffPos025;
    }
    else {
        x1mod = x1 + xDiffPos025;
    }

    let y1mod = y1 + yDiffPos025; // always is positive
    let x2mod = -1;

    if (path.charAt(path.length - 1) == '0') {  // if path ends in 0, then it is added (else subtracted)
        x2mod = x2 + xDiffNeg025;
    }
    else {
        x2mod = x2 - xDiffNeg025;
    }

    let y2mod = y2 - yDiffNeg025; // always is negative

    if (testFeature1) {
        console.log(`x1mod: ${x1mod} y1mod: ${y1mod} x2mod: ${x2mod} y2mod: ${y2mod}`);
    }

    // parent
    makeParentArrowWithMarker(x2mod, y2mod, x1mod, y1mod, idNum);      // reverse coordinates consistent with child to parent and for arrow placement/direction
}


function makeChildArrowWithMarker(x1, y1, x2, y2) {
    let $blueMarker = svg1.add(`marker`,
        {
            markerWidth: '6',
            markerHeight: '6',
            orient: 'auto-start-reverse',
            id: 'arrowhead',
            viewBox: "0 0 10 10",
            refX: "5",
            refY: "5"

        },
        { strokeWidth: '1px', stroke: 'blue', fill: 'blue' }
    );

    let $path = document.createElementNS(Svg.NAMESPACE_URI, 'path');
    //$path.setAttribute('d', `M 0,0 L -${baseArrow},0 L 0,${heightArrow} L ${baseArrow},0 Z`);
    $path.setAttribute('d', `M 0,0 L 10,5 L 0,10 z`);
    $blueMarker.appendChild($path);

    svg1.add(`line`,
        {
            x1: x1,
            y1: y1,
            x2: x2,
            y2: y2,
            'marker-end': `url(#${$blueMarker.id})`
        },
        { strokeWidth: `1px`, stroke: `blue` }
    );
}

function makeParentArrowWithMarker(x1, y1, x2, y2, idNum) {
    let $greenMarker = svg1.add(`marker`,
        {
            markerWidth: '6',
            markerHeight: '6',
            orient: 'auto-start-reverse',
            id: 'arrowhead1',
            viewBox: "0 0 10 10",
            refX: "5",
            refY: "5"
        },
        { strokeWidth: '1px', stroke: 'green', fill: 'green' }
    );

    let $path = document.createElementNS(Svg.NAMESPACE_URI, 'path');

    //$path.setAttribute('d', `M 0,0 L -${baseArrow},0 L 0,${heightArrow} L ${baseArrow},0 Z`);
    $path.setAttribute('d', `M 0,0 L 10,5 L 0,10 z`);
    //$path.setAttribute('d', path);
    $greenMarker.appendChild($path);

    svg1.add(`line`,
        {
            x1: x1,
            y1: y1,
            x2: x2,
            y2: y2,
            id: 'line' + idNum,
            'marker-end': `url(#${$greenMarker.id})`
        },
        { strokeWidth: '1px', stroke: 'green' }
    );

}

const delay = ms => new Promise(resolve => setTimeout(resolve, ms));



const svgText = document.getElementById("circleText");
//svgText.textContent = "Hello";

// needs a loop, since appending a node can filter up
// group all the changes from heapArrayBeforeFilterUp to heapArrayAfterFilterUp (possibly comparing keys during development since filter up process is being reproduced)
// update model with changes because we need to keep track of the cx and cy values (for the next appendNewTreeNode operation, at least), at least
let iteration = 0;

function getIterationValue() {
    return iteration;
}

// iteration is accurate up to 2^53 - 1, so we can safely assume that we have enough integers for anything that the user wants with a 15-element heap - see this for some related details (https://github.com/processing/p5.js/wiki/JavaScript-basics#data-type-number)
// this also covers other increasing usages of integers, elsewhere
async function appendNewTreeNode(treeNode, index, heapArrayParams, heapArrayCopyForView, updateModelCallback, heapArray, updateSiblingNode, AddNodeToTracking, stackOfTxtNodes, mode) {
    console.log("treeNode.path is: " + treeNode.path);    
    const treeNodeGroup = d3.select('#mysvg')
        .append('g')
        .attr('id', 'group' + treeNode.iteration);   // this limits me to 15 nodes (iteration = 14 being highest)

    console.log("path in appendNewTreeNode: " + treeNode.path);
    let path = treeNode.path.slice(0);
    let pathLength = path.length;
    if (pathLength === 0) {
        pathLength = 1;
    }
    if (testFeature1) {
        console.log("path:" + path + " length: " + pathLength);
        console.log(treeNode.key);
    }
    if (testFeature2) {
        console.log("treeNode.iteration:" + treeNode.iteration);
        console.log(path);
        console.log(pathLength);
    }

    let x = topX * ht.get(path);
    let y;
    if (path === "") {
        y = topY * pathLength;
    }
    else {
        y = 2.5 * topY * pathLength;
    }

    treeNode.cx = x;
    treeNode.cy = y;

    treeNode.radius = RADIUS;
    globalGroup1.append('circle')
        .attr('cx', treeNode.cx)
        .attr('cy', treeNode.cy)
        .attr('r', treeNode.radius)
        .attr('stroke', 'red')
        .attr('fill', 'white')
        .attr('id', 'circle' + treeNode.iteration);
    treeNode.circle = document.getElementById('circle' + treeNode.iteration);
    //treeNode.circle.setAttribute('r', 50);

    const circle = treeNode.circle;
    const r = circle.getAttribute('r');

    treeNodeGroup.append('text')
        .attr('x', treeNode.cx)
        .attr('y', treeNode.cy)
        .attr('fill', 'black')
        .attr('id', 'txtElemId' + treeNode.iteration)
        .attr('text-anchor', 'middle')
        .attr('dominant-baseline', 'middle')
        .attr('font-size', squareSideTempVar * 0.4);

    // graphical tracking of iteration used for node, so that it can be removed
    AddNodeToTracking(stackOfTxtNodes, treeNode.iteration);

    const textNode = document.getElementById('txtElemId' + treeNode.iteration);
    textNode.textContent = treeNode.key;
    if (testFeature2) {
        console.log("treeNode.key=" + treeNode.key);
    }
    const svgElem = document.getElementById("mysvg");
    const treeNodeGroupAccessor = document.getElementById('group' + treeNode.iteration);

    // have to update the model with the new cx and cy values for the treeNode, since they are needed for the applyParentToChildLinesToTreeNodes and applyChildToParentLinesToTreeNodes calls (for the lines/arrows to be drawn correctly)
    heapArrayCopyForView[index].cx = treeNode.cx;
    heapArrayCopyForView[index].cy = treeNode.cy;
    //heapArrayCopyForView[index].iteration = treeNode.iteration;
    updateModelCallback(heapArrayCopyForView, heapArray);

    if (testFeature2) {
        console.log("treeNode.cx: " + treeNode.cx);
        console.log("treeNode.cy: " + treeNode.cy);
        console.log(treeNode);
        if (treeNode.Parent != null) {
            console.log("treeNode.Parent.cx: " + treeNode.Parent.cx);
            console.log("treeNode.Parent.cy: " + treeNode.Parent.cy);
        }
    }

    if (path !== "") {
        console.log("value of iteration is: " + iteration);
        //applyParentToChildLinesToTreeNodes(treeNode.Parent.cx, treeNode.Parent.cy, treeNode.cx, treeNode.cy, treeNode.path);
        applyChildToParentLinesToTreeNodes(treeNode.Parent.cx, treeNode.Parent.cy, treeNode.cx, treeNode.cy, treeNode.path, treeNode.iteration);
    }

    // for 6/16 - heapArrayBeforeFilterUp is losing the parent reference on 5 - does it need to be updated in the callback
    if (index >= 1) {
        let parentIndex = Math.trunc((index - 1) / 2);;
        heapArrayCopyForView[index].Parent = heapArrayCopyForView[parentIndex];
    }

    if (heapArrayParams.length >= 2) {
        console.log("HEAPARRAY before filterup: "); // why is parent null here

        console.log("heapArray[heapArrayParams[0]]-");
        console.log(heapArray[heapArrayParams[0]]);
        console.log("heapArray[heapArrayParams[1]]-");
        console.log(heapArray[heapArrayParams[1]]);

        console.log("heapArrayCopyForView[heapArrayParams[0]]-");
        console.log(heapArrayCopyForView[heapArrayParams[0]]);
        console.log("heapArrayCopyForView[heapArrayParams[1]]-");
        console.log(heapArrayCopyForView[heapArrayParams[1]]);

        for (let i = 0; i < heapArrayParams.length; i+=2) {
            // need to delay for 3 x durationOfAnimation + 500 ms (500 ms is necessary - else temp variable box gets messed up), every loop cycle, in parallel with exchangeValues
            exchangeValues(heapArrayCopyForView[heapArrayParams[i]], heapArrayCopyForView[heapArrayParams[i + 1]],
                updateSiblingNode, heapArrayParams[i], heapArrayCopyForView, stackOfTxtNodes);
            await delay((3 * durationOfAnimation * 1000) + GetMode(mode, ADD_OR_REMOVE)); // 500);           
        }

        console.log("HEAPARRAY after filterup: ");
        console.log("heapArray[heapArrayParams[0]]-");
        console.log(heapArray[heapArrayParams[0]]);
        console.log("heapArray[heapArrayParams[1]]-");
        console.log(heapArray[heapArrayParams[1]]);

        console.log("heapArrayCopyForView[heapArrayParams[0]]-");
        console.log(heapArrayCopyForView[heapArrayParams[0]]);
        console.log("heapArrayCopyForView[heapArrayParams[1]]-");
        console.log(heapArrayCopyForView[heapArrayParams[1]]);

        console.log("HEAPARRAY:");
        console.log(heapArray);
        console.log("HEAPARRAY before filter up/copy for view");
        console.log(heapArrayCopyForView);
    }

    if (testFeature3a) {
        console.log("heapArrayCopyForView after appendNewTreeNode:");
        console.log(heapArrayCopyForView);
    }

    iteration++;
}


async function removeTreeNode(maxNode, totalCount, heapArrayParams, heapArrayCopyForView, updateModelCallback, heapArray, updateSiblingNode, GetNodeFromTracking, RemoveNodeFromTracking, stackOfTxtNodes) {
    console.log("Beginning of removeTreeNode.");
    if (testFeature3a) {
        console.log(heapArrayCopyForView);
    }    

    let rootNum = GetNodeFromTracking(stackOfTxtNodes, HeapItem.FIRST);
    let lastNodeNum = GetNodeFromTracking(stackOfTxtNodes, HeapItem.LAST);
        
    // maxNode was passed in and so it never made it into heapArrayCopyForView
    // move the node from the max node spot to the max variable box, then remove it from the heap

    // set up the settling behavior (when and what gets into maximum value box in heap)
    const textA = groupNumbers.append("text")
        .attr('id', 'maxVar' + iteration)
        .attr('font-size', squareSideTempVar * 0.4)
    const numberAnimationA = textA.append('animateMotion')           // this is a number animation, the nodes don't go anywhere
        .attr('dur', durationOfAnimation + 's')
        .attr('fill', 'remove')
        .attr('id', 'moveNumberToMaxVar' + iteration)
        .attr('begin', 'indefinite');

    const motionAnim = document.getElementById('moveNumberToMaxVar' + iteration); 

    motionAnim.addEventListener('endEvent', () => {
        groupNumbers.append("text")
            .attr("x", xMaxVar + squareSideTempVar / 2)
            .attr("y", yMaxVar + squareSideTempVar / 2)
            .attr("text-anchor", "middle")
            .attr("dominant-baseline", "middle")
            .attr('font-size', squareSideTempVar * 0.4)
            .attr('id', 'maxVar' + (iteration + 1))
            .attr("fill", 'black');

        const text = document.getElementById('maxVar' + (iteration + 1));

        // remove all prior text
        if (document.getElementById('maxVar' + lastMaxNode) !== null) {
            document.getElementById('maxVar' + lastMaxNode).remove();
        }

        text.textContent = maxNode.key;
        lastMaxNode = iteration + 1;

        // remove the existing text
        if (totalCount === 1) {
            if (document.getElementById('txtElemId' + rootNum) !== null) {
                document.getElementById('txtElemId' + rootNum).remove();
            }
            if (document.getElementById('circle' + rootNum) !== null) {
                document.getElementById('circle' + rootNum).remove();
            }            
            RemoveNodeFromTracking(stackOfTxtNodes);
        }        
    });

    // get the group
    const treeNodeGroup = d3.select('#mysvg');

    // make the path and append to the group
    treeNodeGroup.append('path')
        .attr('id', 'linePath' + iteration)
        .attr('d', 'M ' + maxNode.cx + " " + maxNode.cy + ' L ' + ' ' + (xMaxVar + squareSideTempVar / 2) + ' ' + (yMaxVar + squareSideTempVar / 2))
        .attr('stroke', 'none');

    // append the text to the group
    const text = treeNodeGroup.append('text')
        .attr('id', 'animationText' + iteration)
        .attr('font-size', squareSideTempVar * 0.4);

    // set the text to the maxNode's key
    const animationText = document.getElementById('animationText' + iteration);
    animationText.textContent = maxNode.key;

    // set elements of the motion (excluding path)
    const numberAnimation = text.append('animateMotion')
        .attr('dur', durationOfAnimation + 's')
        .attr('fill', 'remove')
        .attr('id', 'moveNumberToMaxVar' + (iteration + 1))
        .attr('begin', 'indefinite');

    // append the path to the animation
    numberAnimation.append('mpath')
        .attr('xlink:href', '#linePath' + iteration);

    // move last node to top node
    let nodeToReplace = null;
    if (totalCount > 1) {
        let nodeToReplaceTemp = new ArrayHeapNode(heapArrayCopyForView[totalCount - 1].key,
            heapArrayCopyForView[totalCount - 1].value);
        nodeToReplaceTemp.iteration = heapArrayCopyForView[totalCount - 1].iteration;
        nodeToReplaceTemp.cx = heapArrayCopyForView[totalCount - 1].cx;     
        nodeToReplaceTemp.cy = heapArrayCopyForView[totalCount - 1].cy;
        nodeToReplaceTemp.path = calculatePath(totalCount - 1);

        nodeToReplace = nodeToReplaceTemp;

        // copy the last node's values to the top node's values in the heapArrayCopyForView, 
        // since we are moving the last node to the top node's position (not cx and cy)

        heapArrayCopyForView[0] = new ArrayHeapNode(-1, -1);
        heapArrayCopyForView[0].key = heapArrayCopyForView[totalCount - 1].key;
        heapArrayCopyForView[0].value = heapArrayCopyForView[totalCount - 1].value;
        heapArrayCopyForView[0].iteration = heapArrayCopyForView[totalCount - 1].iteration;

        // parent reference not needed; top node has no parent, and bottom node's parent reference is not copied, bottom
        // reference is nulled out below

        heapArrayCopyForView[0].cx = maxNode.cx;
        heapArrayCopyForView[0].cy = maxNode.cy;

        heapArrayCopyForView[totalCount - 1] = null;

        console.log("heapArrayCopyForView after replaced top node with last:");
        console.log(heapArrayCopyForView);

        // move nodeToReplace to the maxNode's position

        // graph update
        treeNodeGroup.append('path')
            .attr('id', 'linePath' + (iteration + 1))
            .attr('d', 'M ' + nodeToReplace.cx + " " + nodeToReplace.cy + ' L ' + ' ' + maxNode.cx + ' ' + maxNode.cy)
            .attr('stroke', 'none');

        const text2 = treeNodeGroup.append('text')
            .attr('id', 'animationText' + (iteration + 1))
            .attr('font-size', squareSideTempVar * 0.4);

        const animationText2 = document.getElementById('animationText' + (iteration + 1));
        animationText2.textContent = nodeToReplace.key;

        const numberAnimation2 = text2.append('animateMotion')
            .attr('dur', durationOfAnimation + 's')
            .attr('fill', 'remove')
            .attr('id', 'moveLastToTopOfHeap' + iteration)
            .attr('begin', 'moveNumberToMaxVar' + (iteration + 1) + '.end');

        numberAnimation2.append('mpath')
            .attr('xlink:href', '#linePath' + (iteration + 1));
        const motionAnim2 = document.getElementById('moveLastToTopOfHeap' + iteration);
        motionAnim2.addEventListener('endEvent', () => {
            treeNodeGroup.append('text')    // gr
                .attr('x', maxNode.cx)
                .attr('y', maxNode.cy)
                .attr("text-anchor", "middle")
                .attr("dominant-baseline", "middle")
                .attr('font-size', squareSideTempVar * 0.4)
                .attr('id', 'maxNodeDest' + iteration)
                .attr("fill", 'black');

            //const textNode2 = document.getElementById('txtElemId' + 11);
            //textNode2.textContent = treeNode2.key;                
            const textNode2 = document.getElementById('txtElemId' + rootNum); // 'txtElemId' + maxNode.iteration);     // retrieve old text id and overwrite
            textNode2.textContent = nodeToReplace.key;

            // remove the node
            if (document.getElementById('txtElemId' + lastNodeNum) !== null) {
                document.getElementById('txtElemId' + lastNodeNum).remove();
            }
            if (document.getElementById('circle' + lastNodeNum) !== null) {
                document.getElementById('circle' + lastNodeNum).remove();
            }

            if (document.getElementById('linePath' + lastNodeNum) !== null) {  // do it again if failed first time
                document.getElementById('linePath' + lastNodeNum).remove();
            }
            if (document.getElementById('line' + lastNodeNum) !== null) {
                document.getElementById('line' + lastNodeNum).remove();
            }

            RemoveNodeFromTracking(stackOfTxtNodes);
        });
    }

    const animationA = document.getElementById('moveNumberToMaxVar' + iteration);
    animationA.beginElement();

    const animationB = document.getElementById('moveNumberToMaxVar' + (iteration + 1));
    animationB.beginElement();

    // filter down - must begin the max node and last-node-to-top operations before we can filter down
    if (heapArrayParams.length > 0) {
        for (let i = 0; i < heapArrayParams.length; i += 2) {                                                                                   // next parameter down
            if (i === 0) {      // NW in terms of lookup function
                await delay(2 * durationOfAnimation * 1000);        // wait for the max node to be moved to the max variable box, then wait for the last node to be moved to the top of the heap
            }
            exchangeValuesFilterDown(heapArrayCopyForView[heapArrayParams[i]], heapArrayCopyForView[heapArrayParams[i + 1]], updateSiblingNode, heapArrayParams[i], heapArrayParams[i + 1], heapArrayCopyForView, stackOfTxtNodes, iteration);
            await delay((3 * durationOfAnimation * 1000) + GetMode(USER_INPUT, ADD_OR_REMOVE)); // 500       
        }

        // if the last index of the filter down does not go all the way to the bottom of the heap, then we need to fix
        // one child (both children does not seem to be possible)
        if (heapArrayParams[heapArrayParams.length - 1] < Math.trunc((totalCount - 1) / 2)) {
            fixParentReference(totalCount - 1, heapArrayCopyForView);
        }
    }
    else {
        if (totalCount === 3 && heapArrayCopyForView[0].key > heapArrayCopyForView[1].key) {     // 5-3-4 case with removal 
            fixParentReference(2, heapArrayCopyForView);
        }
    }
    iteration += 3;
    console.log("End of removeTreeNode.");
}

// iteration - look at renaming, explicitly declaring in exchangeValues
// SVG animations custom events begin - google search
// treeNode's values are being exchanged with treeeNode2, which is its parent
let iterationCount = 0;
function exchangeValues(treeNode, treeNode2, updateSiblingNode, childIndex, heapArrayCopyForView, stackOfTxtNodes) {
    // settling
    const textA = groupNumbers.append("text")
        .attr('id', 'tempVar' + iterationCount)
        .attr('font-size', squareSideTempVar * 0.4)
    //.attr('x', treeNode.cx)   // messes up the animation when either or both are enabled 
    //.attr('y', treeNode.cy);
    //const text2 = document.getElementById('tempVar2');          // if we place all numbers in a single group, that will move all, so one group per number with this model
    //text2.textContent = treeNode.key;                           // we will need to remove nodes, too - 

    const numberAnimationA = textA.append('animateMotion')           // this is a number animation, the nodes don't go anywhere
        .attr('dur', durationOfAnimation + 's')
        .attr('fill', 'remove')
        .attr('id', 'moveNumberToTempVar' + iterationCount)
        .attr('begin', 'indefinite');

    // append the path to the animation
    //numberAnimation.append(`mpath`)
    //    .attr(`xlink:href`, '#linePath');

    const motionAnim = document.getElementById('moveNumberToTempVar' + iterationCount);
    // at the end of the animation, set the text of the temp variable to the treenode whose path was previously set, this is so that it appears on top of
    // the rectangle

    motionAnim.addEventListener('endEvent', () => {         
        groupNumbers.append("text")
            .attr("x", xTempVar + squareSideTempVar / 2)
            .attr("y", yTempVar + squareSideTempVar / 2)
            .attr("text-anchor", "middle")
            .attr("dominant-baseline", "middle")
            .attr('font-size', squareSideTempVar * 0.4)
            .attr('id', 'tempVar' + (iterationCount + 1))
            .attr("fill", 'black');
        const text = document.getElementById('tempVar' + (iterationCount + 1));
        text.textContent = treeNode.key;
    });

    if (testFeature2) {
        console.log("treeNode.key is: " + treeNode.key); 
        console.log("treenode2.key is: " + treeNode2.key);
        console.log("uniqueID=" + iterationCount);
    }

    const treeNodeGroup = d3.select('#mysvg')
        .append('g')
        .attr('id', 'group' + (iterationCount + 15));
    if (testFeature2) {
        console.log(treeNodeGroup);
    }

    // save treeNode in temp
    // animation
//    let temp = treeNode;  // data structure update

    // graph update
    treeNodeGroup.append('path')
        .attr('id', 'linePath' + iterationCount)
        .attr('d', 'M ' + treeNode.cx + " " + treeNode.cy + ' L ' + ' ' + (xTempVar + squareSideTempVar / 2) + ' ' + (yTempVar + squareSideTempVar / 2))
        .attr('stroke', 'none');

    const text = treeNodeGroup.append('text')
        .attr('id', 'animationText' + iterationCount)
        .attr('font-size', squareSideTempVar * 0.4);
    if (testFeature2) {
        console.log(treeNodeGroup);
        console.log(text);
    }
    const animationText = document.getElementById('animationText' + iterationCount);
    if (testFeature2) {
        console.log(animationText);
    }

    animationText.textContent = treeNode.key;

    const numberAnimation = text.append('animateMotion')
        .attr('dur', durationOfAnimation + 's')
        .attr('fill', 'remove')
        .attr('id', 'moveNumberToTempVar' + (iterationCount + 1))
        //.attr('from', '0')
        //.attr('to', '100')
        .attr('begin', 'indefinite');

    numberAnimation.append('mpath')
        .attr('xlink:href', '#linePath' + iterationCount);

    // move treeNode2 to treeNode
    treeNodeGroup.append('path')
        .attr('id', 'linePath' + (iterationCount + 1))
        .attr('d', 'M ' + treeNode2.cx + " " + treeNode2.cy + ' L ' + ' ' + treeNode.cx + ' ' + treeNode.cy)
        .attr('stroke', 'none');

    const text2 = treeNodeGroup.append('text')
        .attr('id', 'animationText' + (iterationCount + 1))
        .attr('font-size', squareSideTempVar * 0.4);

    const animationText2 = document.getElementById('animationText' + (iterationCount + 1));
    animationText2.textContent = treeNode2.key;

    const numberAnimation2 = text2.append('animateMotion')
        .attr('dur', durationOfAnimation + 's')
        .attr('fill', 'remove')
        .attr('id', 'moveTreeNode2ToTreeNode' + iterationCount)
        .attr('begin', 'moveNumberToTempVar' + (iterationCount + 1) + '.end');

    numberAnimation2.append('mpath')
        .attr('xlink:href', '#linePath' + (iterationCount + 1));

    const motionAnim2 = document.getElementById('moveTreeNode2ToTreeNode' + iterationCount);
    motionAnim2.addEventListener('endEvent', () => {
        console.log("treeNode.cx=" + treeNode.cx + " treeNode.cy=" + treeNode.cy);
        treeNodeGroup.append('text')    // gr
            .attr('x', treeNode.cx)
            .attr('y', treeNode.cy)
            .attr("text-anchor", "middle")
            .attr("dominant-baseline", "middle")
            .attr('font-size', squareSideTempVar * 0.4)
            .attr('id', 'treeNodeDest' + iterationCount)
            .attr("fill", 'black');

        //const textNode2 = document.getElementById('txtElemId' + 11);
        //textNode2.textContent = treeNode2.key;                
        const childItemNodeNum = stackOfTxtNodes[childIndex];  
        const textNode = document.getElementById('txtElemId' + childItemNodeNum);     // retrieve old text id and overwrite
        textNode.textContent = treeNode2.key;
    });
    //// const text2 = document.getElementById('tempVar' + 16);
    //// text2.textContent = treeNode2.key;
    
    // move temp to treeNode2 - need to keep track of internally swapped nodes
//    treeNode2 = temp;  // data structure update

    // graph update
    treeNodeGroup.append('path')
        .attr('id', 'linePath' + (iterationCount + 2))
        .attr('d', 'M ' + (xTempVar + squareSideTempVar / 2) + " " + (yTempVar + squareSideTempVar / 2) + ' L ' + ' ' + treeNode2.cx + ' ' + treeNode2.cy)
        .attr('stroke', 'none');

    const text3 = treeNodeGroup.append('text')
        .attr('id', 'animationText' + (iterationCount + 2))
        .attr('font-size', squareSideTempVar * 0.4);

    const animationText3 = document.getElementById('animationText' + (iterationCount + 2));
    animationText3.textContent = animationText.textContent; // document.getElementById('tempVar16').textContent;

    const numberAnimation3 = text3.append('animateMotion')
        .attr('dur', durationOfAnimation + 's')
        .attr('fill', 'remove')
        .attr('id', 'moveTempToTreeNode2' + iterationCount)
        .attr('begin', 'moveTreeNode2ToTreeNode' + iterationCount + '.end');

    numberAnimation3.append('mpath')
        .attr('xlink:href', '#linePath' + (iterationCount + 2));

    const motionAnim3 = document.getElementById('moveTempToTreeNode2' + iterationCount);
    motionAnim3.addEventListener('endEvent', () => {
        treeNodeGroup.append('text')    // gr
            .attr('x', treeNode2.cx)
            .attr('y', treeNode2.cy)
            .attr("text-anchor", "middle")
            .attr("dominant-baseline", "middle")
            .attr('font-size', squareSideTempVar * 0.4)
            .attr('id', 'treeNod2Dest') // + i)
            .attr("fill", 'black');

        //const textNode2 = document.getElementById('txtElemId' + 11);
        //textNode2.textContent = treeNode2.key;
        if (testFeature2) {
            console.log("treeNode2.iteration:" + treeNode2.iteration);
            console.log("treeNode2.key:" + treeNode2.key);
        }

        let parentIndex = Math.trunc((childIndex - 1) / 2);
        const parentItemNodeNum = stackOfTxtNodes[parentIndex];
        const textNode = document.getElementById('txtElemId' + parentItemNodeNum);
        textNode.textContent = animationText.textContent //document.getElementById('tempVarText').textContent;
        if (testFeature2) {
            console.log('textNode.textContent: ' + textNode.textContent);
        }

        // clear the temp variable after copy to treeNode2
        groupNumbers.append("text")
            .attr("x", xTempVar + squareSideTempVar / 2)
            .attr("y", yTempVar + squareSideTempVar / 2)
            .attr("text-anchor", "middle")
            .attr("dominant-baseline", "middle")
            .attr('font-size', squareSideTempVar * 0.4)
            .attr('id', 'tempVar' + (iterationCount + 1))
            .attr("fill", 'black');
        const text = document.getElementById('tempVar' + (iterationCount + 1));
        text.textContent = "";

        exchangeUnderlyingValues(treeNode, treeNode2);
        updateSiblingNode(childIndex, heapArrayCopyForView);
    });

    const animationA = document.getElementById('moveNumberToTempVar' + iterationCount);
    animationA.beginElement();
    const animationB = document.getElementById('moveNumberToTempVar' + (iterationCount +1));
    animationB.beginElement();
//    document.addEventListener('startMoveToTempVar', (e) => {
//        const animation = document.getElementById('moveNumberToTempVar');
//        animation.beginElement();
//    });
    //    motionAnim.dispatchEvent(new CustomEvent("startMoveToTempVar"));
    iterationCount += 3;
}


function exchangeValuesFilterDown(treeNode, treeNode2, updateSiblingNode, parentIndex, childIndex, heapArrayCopyForView, stackOfTxtNodes, iteration) {
    let tempKey = treeNode.key;  // avoid using animation text

    // settling
    const textA = groupNumbers.append("text")
        .attr('id', 'tempVar' + iterationCount)
        .attr('font-size', squareSideTempVar * 0.4)
    //.attr('x', treeNode.cx)   // messes up the animation when either or both are enabled 
    //.attr('y', treeNode.cy);
    //const text2 = document.getElementById('tempVar2');          // if we place all numbers in a single group, that will move all, so one group per number with this model
    //text2.textContent = treeNode.key;                           // we will need to remove nodes, too - 

    const numberAnimationA = textA.append('animateMotion')           // this is a number animation, the nodes don't go anywhere
        .attr('dur', durationOfAnimation + 's')
        .attr('fill', 'remove')
        .attr('id', 'moveNumberToTempVarA' + iterationCount)
        .attr('begin', 'indefinite');

    // append the path to the animation
    //numberAnimation.append(`mpath`)
    //    .attr(`xlink:href`, '#linePath');

    const motionAnim = document.getElementById('moveNumberToTempVarA' + iterationCount);
    // at the end of the animation, set the text of the temp variable to the treenode whose path was previously set, this is so that it appears on top of
    // the rectangle

    motionAnim.addEventListener('endEvent', () => {
        groupNumbers.append("text")
            .attr("x", xTempVar + squareSideTempVar / 2)
            .attr("y", yTempVar + squareSideTempVar / 2)
            .attr("text-anchor", "middle")
            .attr("dominant-baseline", "middle")
            .attr('font-size', squareSideTempVar * 0.4)
            .attr('id', 'tempVar' + (iterationCount + 1))
            .attr("fill", 'black');
        const text = document.getElementById('tempVar' + (iterationCount + 1));
        text.textContent = treeNode.key;
    });

    if (testFeature2) {
        console.log("treeNode.key is: " + treeNode.key);
        console.log("treenode2.key is: " + treeNode2.key);
        console.log("uniqueID=" + iterationCount);
    }

    // animation behavior for treeNode
    const treeNodeGroup = d3.select('#mysvg')
        .append('g')
        .attr('id', 'group' + (iterationCount + 15));
    if (testFeature2) {
        console.log(treeNodeGroup);
    }

    // save treeNode in temp
    // animation
    //    let temp = treeNode;  // data structure update

    // graph update
    treeNodeGroup.append('path')
        .attr('id', 'linePathA' + iterationCount)
        .attr('d', 'M ' + treeNode.cx + " " + treeNode.cy + ' L ' + ' ' + (xTempVar + squareSideTempVar / 2) + ' ' + (yTempVar + squareSideTempVar / 2))
        .attr('stroke', 'none');

    const text1 = treeNodeGroup.append('text')
        .attr('id', 'animationTextA' + iterationCount)      // this conflicts with the other animationText id, so we need to make it unique
        .attr('font-size', squareSideTempVar * 0.4);
    if (testFeature2) {
        console.log(treeNodeGroup);
        console.log(text1);
    }
    const animationText1 = document.getElementById('animationTextA' + iterationCount);
    if (testFeature2) {
        console.log(animationText1);
    }

    animationText1.textContent = treeNode.key;

    const numberAnimation = text1.append('animateMotion')
        .attr('dur', durationOfAnimation + 's')
        .attr('fill', 'remove')
        .attr('id', 'moveNumberToTempVarA' + (iterationCount + 1))
        //.attr('from', '0')
        //.attr('to', '100')
        .attr('begin', 'indefinite');

    numberAnimation.append('mpath')
        .attr('xlink:href', '#linePathA' + iterationCount);


    // move treeNode2 to treeNode

    // graph update
    treeNodeGroup.append('path')
        .attr('id', 'linePathA' + (iterationCount + 1))
        .attr('d', 'M ' + treeNode2.cx + " " + treeNode2.cy + ' L ' + ' ' + treeNode.cx + ' ' + treeNode.cy)
        .attr('stroke', 'none');

    const text2 = treeNodeGroup.append('text')
        .attr('id', 'animationTextB' + (iterationCount + 1))
        .attr('font-size', squareSideTempVar * 0.4);

    const animationText2 = document.getElementById('animationTextB' + (iterationCount + 1));
    animationText2.textContent = treeNode2.key;

    const numberAnimation2 = text2.append('animateMotion')
        .attr('dur', durationOfAnimation + 's')
        .attr('fill', 'remove')
        .attr('id', 'moveTreeNode2ToTreeNode' + iterationCount)
        .attr('begin', 'moveNumberToTempVarA' + (iterationCount + 1) + '.end');

    numberAnimation2.append('mpath')
        .attr('xlink:href', '#linePathA' + (iterationCount + 1));

    const motionAnim2 = document.getElementById('moveTreeNode2ToTreeNode' + iterationCount);
    motionAnim2.addEventListener('endEvent', () => {
        console.log("treeNode.cx=" + treeNode.cx + " treeNode.cy=" + treeNode.cy);
        treeNodeGroup.append('text')    // gr
            .attr('x', treeNode.cx)
            .attr('y', treeNode.cy)
            .attr("text-anchor", "middle")
            .attr("dominant-baseline", "middle")
            .attr('font-size', squareSideTempVar * 0.4)
            .attr('id', 'treeNodeDest' + iterationCount)
            .attr("fill", 'black');

        //const textNode2 = document.getElementById('txtElemId' + 11);
        //textNode2.textContent = treeNode2.key;                
        const parentItemNodeNum = stackOfTxtNodes[parentIndex];
        const textNode = document.getElementById('txtElemId' + parentItemNodeNum);     // retrieve old text id and overwrite
        textNode.textContent = treeNode2.key;
    });

    //// const text2 = document.getElementById('tempVar' + 16);
    //// text2.textContent = treeNode2.key;

    // move temp to treeNode2 - need to keep track of internally swapped nodes
    //    treeNode2 = temp;  // data structure update

    // graph update
    treeNodeGroup.append('path')
        .attr('id', 'linePathA' + (iterationCount + 2))
        .attr('d', 'M ' + (xTempVar + squareSideTempVar / 2) + " " + (yTempVar + squareSideTempVar / 2) + ' L ' + ' ' + treeNode2.cx + ' ' + treeNode2.cy)
        .attr('stroke', 'none');

    const text3 = treeNodeGroup.append('text')
        .attr('id', 'animationTextA' + (iterationCount + 2))
        .attr('font-size', squareSideTempVar * 0.4);

    const animationText3 = document.getElementById('animationTextA' + (iterationCount + 2));
    animationText3.textContent = tempKey; // document.getElementById('tempVar16').textContent;

    const numberAnimation3 = text3.append('animateMotion')
        .attr('dur', durationOfAnimation + 's')
        .attr('fill', 'remove')
        .attr('id', 'moveTempToTreeNode2' + iterationCount)
        .attr('begin', 'moveTreeNode2ToTreeNode' + iterationCount + '.end');

    numberAnimation3.append('mpath')
        .attr('xlink:href', '#linePathA' + (iterationCount + 2));
    const motionAnim3 = document.getElementById('moveTempToTreeNode2' + iterationCount);
    motionAnim3.addEventListener('endEvent', () => {
        treeNodeGroup.append('text')    // gr
            .attr('x', treeNode2.cx)
            .attr('y', treeNode2.cy)
            .attr("text-anchor", "middle")
            .attr("dominant-baseline", "middle")
            .attr('font-size', squareSideTempVar * 0.4)
            .attr('id', 'treeNode2Dest') // + i)
            .attr("fill", 'black');

        //const textNode2 = document.getElementById('txtElemId' + 11);
        //textNode2.textContent = treeNode2.key;
        if (testFeature2) {
            console.log("treeNode2.iteration:" + treeNode2.iteration);
            console.log("treeNode2.key:" + treeNode2.key);
        }

        const childItemNodeNum = stackOfTxtNodes[childIndex];
        const textNode = document.getElementById('txtElemId' + childItemNodeNum);
        textNode.textContent = tempKey; //document.getElementById('tempVarText').textContent;
        if (testFeature2) {
            console.log('textNode.textContent: ' + textNode.textContent);
        }

        // clear the temp variable after copy to treeNode2
        groupNumbers.append("text")
            .attr("x", xTempVar + squareSideTempVar / 2)
            .attr("y", yTempVar + squareSideTempVar / 2)
            .attr("text-anchor", "middle")
            .attr("dominant-baseline", "middle")
            .attr('font-size', squareSideTempVar * 0.4)
            .attr('id', 'tempVar' + (iterationCount + 1))
            .attr("fill", 'black');
        const text = document.getElementById('tempVar' + (iterationCount + 1));
        text.textContent = "";

        console.log("Before exchanging values, in exchangeValuesFilterDown, heapArrayCopyForView:");
        console.log(heapArrayCopyForView);  

        exchangeUnderlyingValues(treeNode2, treeNode);
        updateSiblingNode(childIndex, heapArrayCopyForView);

        console.log("After exchanging values, in exchangeValuesFilterDown, heapArrayCopyForView:");
        console.log(heapArrayCopyForView);  
    });

    const animationA = document.getElementById('moveNumberToTempVarA' + iterationCount);
    animationA.beginElement();
    const animationB = document.getElementById('moveNumberToTempVarA' + (iterationCount + 1));
    animationB.beginElement();

    iterationCount += 3;
}

// treeNode will become the child of treeNode2 once this function completes, if it's not already
function exchangeUnderlyingValues(treeNode, treeNode2) {
    let tempKey = treeNode.key;
    let tempValue = treeNode.value;
    let tempIteration = treeNode.iteration;

    treeNode.key = treeNode2.key;
    treeNode.value = treeNode2.value;
    treeNode.iteration = treeNode2.iteration;
    //treeNode.Parent = treeNode2.Parent;

    treeNode2.key = tempKey;
    treeNode2.value = tempValue;
    treeNode2.iteration = tempIteration;
    //treeNode2.Parent = tempParent;

    treeNode.Parent = treeNode2;
}

class HeapNodeOld {
    constructor(key, value) {
        if (!Number.isInteger(key) || key <= 0) {
            throw new TypeError("Key must be a non-negative integer");
        }
        if (typeof value !== 'string' || value.trim() === '') {
            throw new TypeError("value must be a non-empty string")
        }
        this.key = key;
        this.value = value;
        this.path = '';

        this.Parent = {};
        this.LeftNode = {};
        this.RightNode = {};

        this.cx = -1;
        this.cy = -1;
        this.radius = -1;
    }

    printHeapNode() {
        console.log("Key: " + this.key + " Value: " + this.value + " Path: " + this.path + " Parent: " + this.Parent.key + " LeftNode: " + this.LeftNode.key + " RightNode: " + this.RightNode.key);
    };
}


function printLegendAndRectangles() {
    // label at beginning 
    const myGroup = document.getElementById("mygroup");
    const textElem0 = document.createElementNS("http://www.w3.org/2000/svg", "text");
    const textNode0 = document.createTextNode("Max Heap Legend:");
    textElem0.setAttribute('font-weight', 'bold');
    textElem0.setAttribute('font-size', 18);
    textElem0.appendChild(textNode0);
    myGroup.appendChild(textElem0);

    // Heap node with key
    const circleElem1 = document.createElementNS("http://www.w3.org/2000/svg", "circle");
    circleElem1.setAttribute('cx', 10);
    circleElem1.setAttribute('cy', 20);
    circleElem1.setAttribute('r', 10);
    circleElem1.setAttribute('id', 'legendCircle');
    circleElem1.setAttribute('strokeWidth', '1px');
    circleElem1.setAttribute('stroke', 'red');
    circleElem1.setAttribute('fill', 'white');
    myGroup.appendChild(circleElem1);

    const textElem1 = document.createElementNS("http://www.w3.org/2000/svg", "text");
    const textNode1 = document.createTextNode('x');
    textElem1.setAttribute('x', 10);
    textElem1.setAttribute('y', 20);
    textElem1.setAttribute('text-anchor', 'middle');
    textElem1.setAttribute('dominant-baseline', 'middle');
    textElem1.appendChild(textNode1);
    myGroup.appendChild(textElem1);

    const textElem2 = document.createElementNS("http://www.w3.org/2000/svg", "text");
    const textNode2 = document.createTextNode("- Heap node with key");
    textElem2.setAttribute('x', 27);
    textElem2.setAttribute('y', 26);
    textElem2.appendChild(textNode2);
    myGroup.appendChild(textElem2);

    // Parent to child reference
    makeChildArrowWithMarker(0, 45, 25, 45);

    const textElem3 = document.createElementNS("http://www.w3.org/2000/svg", "text");
    const textNode3 = document.createTextNode(" - Parent to child reference");
    textElem3.setAttribute('x', 39);
    textElem3.setAttribute('y', 48);
    textElem3.appendChild(textNode3);
    myGroup.append(textElem3);

    // Child to parent reference
    makeParentArrowWithMarker(0, 65, 25, 65, 0, -1);

    const textElem4 = document.createElementNS("http://www.w3.org/2000/svg", "text");
    const textNode4 = document.createTextNode(" - Child to parent reference");
    textElem4.setAttribute('x', 39);
    textElem4.setAttribute('y', 69);
    textElem4.appendChild(textNode4);
    myGroup.append(textElem4);

    // Maximum value
    globalGroup1.append('rect')
        .attr('x', 5)
        .attr('y', 80)
        .attr('width', squareSideTempVar * 0.35)
        .attr('height', squareSideTempVar * 0.35)
        .attr('stroke', 'black')
        .attr('strokeWidth', '2px')
        .attr('fill', 'white');

    globalGroup1.append('text')
        .attr('x', 5 + (squareSideTempVar * 0.1))
        .attr('y', 80 + (squareSideTempVar * 0.21))
        .attr('id', 'maxVarTextLegend');

    const maxVarTextLegend = document.getElementById('maxVarTextLegend');
    maxVarTextLegend.textContent = 'y';

    globalGroup1.append('text')
        .attr('x', 39)
        .attr('y', 93)
        .attr('id', 'maxValue');
    const maxValue = document.getElementById('maxValue');
    maxValue.textContent = "- Maximum value in heap"

    // Temporary variable
    globalGroup1.append('rect')
        .attr('x', 5)
        .attr('y', 105)
        .attr('width', squareSideTempVar * 0.35)
        .attr('height', squareSideTempVar * 0.35)
        .attr('stroke', 'brown')
        .attr('strokeWidth', '2px')
        .attr('fill', 'white');

    globalGroup1.append('text')
        .attr('x', 5 + (squareSideTempVar * 0.1))
        .attr('y', 105 + (squareSideTempVar * 0.21))
        .attr('id', 'tempVarTextLegend');

    const temptext = document.getElementById('tempVarTextLegend');
    temptext.textContent = 'z';

    globalGroup1.append('text')
        .attr('x', 39)
        .attr('y', 118)
        .attr('id', 'tempVarExplanatoryText');
    const tempExplanatoryText = document.getElementById('tempVarExplanatoryText');
    tempExplanatoryText.textContent = " - Temporary variable";


    // rect for maximum values
    globalGroup1.append('rect')
        .attr('x', xMaxVar)
        .attr('y', yMaxVar)
        .attr('width', squareSideTempVar)
        .attr('height', squareSideTempVar)
        .attr('stroke', 'black')
        .attr('strokeWidth', '2px')
        .attr('fill', 'white');

    // rect for temporary values
    globalGroup1.append('rect')
        .attr('x', xTempVar)
        .attr('y', yTempVar)
        .attr('width', squareSideTempVar)
        .attr('height', squareSideTempVar)
        .attr('stroke', 'brown')
        .attr('strokeWidth', '2px')
        .attr('fill', 'white');

}


class HeapEvent {
    constructor(eventType, eventData, eventTotalCountPreUpdate, eventParams, heapArrayCopyForView, callback, heapArray, callbackForTxtNodePrimaryOperation, callbackForTxtNodeRemoval, stackOfTxtNodes, mode) {
        this.eventType = eventType;
        this.eventData = eventData;
        this.eventTotalCountPreUpdate = eventTotalCountPreUpdate;
        this.eventParams = eventParams;
        this.heapArrayCopyForView = heapArrayCopyForView;
        this.callback = callback;
        this.heapArray = heapArray;
        this.callbackForTxtNodePrimaryOperation = callbackForTxtNodePrimaryOperation;   // add or get
        this.callbackForTxtNodeRemoval = callbackForTxtNodeRemoval;
        this.stackOfTxtNodes = stackOfTxtNodes;
        this.mode = mode;
    }
}


class Observer {
    Notify(heapEvent) {

    }
}

// this is stateless, but there could be multiple views
class HeapView extends Observer {
    constructor() {
        super();
    }

    Notify(heapEvent) {
        switch (heapEvent.eventType) {
            case "add":
                if (testFeature2b) {
                    console.log("eventType: " + heapEvent.eventType + " key: " + heapEvent.eventData.key + " value: " + heapEvent.eventData.value + " eventTotalCountPreUpdate: " + heapEvent.eventTotalCountPreUpdate + " eventParams length: " + heapEvent.eventParams.length);
                    for (let i = 0; i < heapEvent.eventParams.length; i++) {
                        console.log(" heapEvent.eventParams[" + i + "]:" + heapEvent.eventParams[i]);
                    }
                    console.log(heapEvent.heapArrayCopyForView);
                    console.log(heapEvent.callback);
                    console.log(heapEvent.heapArray);
                }
                let node = new ArrayHeapNode(heapEvent.eventData.key, heapEvent.eventData.value);
                node.iteration = heapEvent.eventData.iteration;
                node.path = calculatePath(heapEvent.eventTotalCountPreUpdate);
                if (testFeature2b) {
                    console.log("path in notify: " + node.path);
                }

                // need index to determine parent, since array is fixed-size, but should null out array on removeMax operations, too
                if (heapEvent.eventTotalCountPreUpdate == 0) {
                    node.Parent = null;

                    if (testFeature2b) {
                        console.log("heapEvent.heapArray[heapEvent.eventTotalCountPreUpdate].Parent"); //.Parent.cx: " +
                        console.log(heapEvent.heapArray[heapEvent.eventTotalCountPreUpdate].Parent); //;.Parent.cx);
                        console.log("heapEvent.heapArray[heapEvent.eventTotalCountPreUpdate]"); //.Parent.cy: " +
                        console.log(heapEvent.heapArray[heapEvent.eventTotalCountPreUpdate]); //.Parent.cy);
                    }
                }
                else {
                    let parentIndex = Math.trunc((heapEvent.eventTotalCountPreUpdate - 1) / 2);
                    console.log("parentIndex is: " + parentIndex);
                    node.Parent = heapEvent.heapArrayCopyForView[parentIndex];
                    if (testFeature2b) {
                        console.log(heapEvent.heapArray[parentIndex])
                        console.log(node.Parent);
                        console.log(node);
                    }
                    if (testFeature2b) {
                        console.log("heapEvent.heapArray[parentIndex].cx: " + heapEvent.heapArray[parentIndex].cx);
                        for (let prop in heapEvent.heapArray[parentIndex]) {
                            console.log(prop + " " + heapEvent.heapArray[parentIndex][prop])
                        }
                        console.log("heapEvent.heapArray[parentIndex].cy: " + heapEvent.heapArray[parentIndex].cy);
                        console.log("node.Parent.cx: " + node.Parent.cx);
                        console.log("node.Parent.cy: " + node.Parent.cy);
                        console.log("node.cx: " + node.cx);
                        console.log("node.cy: " + node.cy);
                    }
                }

                appendNewTreeNode(node, heapEvent.eventTotalCountPreUpdate, heapEvent.eventParams, heapEvent.heapArrayCopyForView, heapEvent.callback, heapEvent.heapArray, fixParentReference, heapEvent.callbackForTxtNodePrimaryOperation, heapEvent.stackOfTxtNodes, heapEvent.mode);
                break;

            case "removeMax":
                let nodeToRemove = new ArrayHeapNode(heapEvent.eventData.key, heapEvent.eventData.value);  
                nodeToRemove.iteration = heapEvent.eventData.iteration;
                nodeToRemove.cx = heapEvent.eventData.cx;
                nodeToRemove.cy = heapEvent.eventData.cy;

                // no need to pass mode here, since it is hard-coded to USER_INPUT 
                removeTreeNode(nodeToRemove, heapEvent.eventTotalCountPreUpdate, heapEvent.eventParams, heapEvent.heapArrayCopyForView, heapEvent.callback, heapEvent.heapArray, fixParentReference, heapEvent.callbackForTxtNodePrimaryOperation, heapEvent.callbackForTxtNodeRemoval, heapEvent.stackOfTxtNodes);
                break;

            default:
                console.log("Unknown event type: " + heapEvent.eventType);
                break;
        }
    }
}

function calculatePath(countOfItems) {
    switch (countOfItems) {
        case 0:
            return '';
        case 1:
            return '0';
        case 2:
            return '1';
        case 3:
            return '00';
        case 4:
            return '01';
        case 5:
            return '10';
        case 6:
            return '11';
        case 7:
            return '000';
        case 8:
            return '001';
        case 9:
            return '010';
        case 10:
            return '011';
        case 11:
            return '100';
        case 12:
            return '101';
        case 13:
            return '110';
        case 14:
            return '111';
    }
}


